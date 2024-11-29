
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using OE.MQ.IBM.XMS;
using PieHandlerService.Core.Interface;
using PieHandlerService.Core.Interfaces;
using PieHandlerService.Core.Models;
using PieHandlerService.Infrastructure.Services.Messaging;
using PieHandlerService.Infrastructure.Services.Messaging.Interfaces;
using System.Threading.Channels;
using PieHandlerService.Infrastructure.Extensions;


namespace PieHandlerService.Infrastructure.Services.Helper;

public sealed class OBChannelHandlerService(IHostApplicationLifetime lifetime, IMapper mapper, IServiceScopeFactory serviceScopeFactory, ILogger<OBChannelHandlerService> logger, IOptions<MqConfiguration> ibmMqConfigurationOptions,
    IOptions<ChannelConfig> channelConfig) : BackGroundWorkerService(lifetime), IOBChannelHandlerService
{
    private readonly ILogger<OBChannelHandlerService> _logger = logger ?? NullLogger<OBChannelHandlerService>.Instance;

    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));

    private readonly ChannelConfig _channelConfig = channelConfig?.Value ?? new();

    private readonly MqConfiguration _mqConfiguration = ibmMqConfigurationOptions.Value ?? throw new ArgumentNullException(nameof(ibmMqConfigurationOptions));

    private IPieOrderRepository? _pieOrderRepository;

    private readonly Channel<PieResponseMessage> _channel = CreateMessageChannel(channelConfig?.Value ?? new ChannelConfig(), false);
    private readonly Channel<PieResponseMessage> _priorityChannel = CreateMessageChannel(channelConfig?.Value ?? new ChannelConfig(), true);

    private CancellationToken? _cancellationToken;

    private int _priorityCounter;

    private IMqOrderPublisher? _mqOrderPublisher;


    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"{nameof(OBChannelHandlerService)} started: ");
        using var topLevelScope = _serviceScopeFactory.CreateScope();
        CreateMqOrderPublisher(topLevelScope, cancellationToken);
        InjectDependencies(topLevelScope);
        return base.StartAsync(cancellationToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"{nameof(OBChannelHandlerService)} stopped:");
        return base.StopAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"{nameof(OBChannelHandlerService)} {nameof(ExecuteAsync)} started.");

        _cancellationToken = cancellationToken;
        await AwaitApplicationStarted(cancellationToken);

        while (!cancellationToken.IsCancellationRequested)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            await ProcessPrioritizedMessageBatch(scope, cancellationToken);
            await ProcessNormalMessage(scope, cancellationToken);
            await DelayTime(cancellationToken);
        }

        _logger.LogInformation($"{nameof(OBChannelHandlerService)}  ExecuteAsync Terminating.");
    }

    private void InjectDependencies(IServiceScope serviceScope)
    {
        _pieOrderRepository = serviceScope.ServiceProvider.GetRequiredService<IPieOrderRepository>() ?? throw new ArgumentNullException(nameof(_pieOrderRepository));
    }

    private static Channel<PieResponseMessage> CreateMessageChannel(ChannelConfig options, bool prioritizedChannel)
    {
        return options.Capacity < 0
            ? Channel.CreateUnbounded<PieResponseMessage>()
            : Channel.CreateBounded<PieResponseMessage>(
                new BoundedChannelOptions(DecideCapacity())
                {
                    FullMode = options.FullMode,
                    SingleReader = true,
                    SingleWriter = false
                });

        int DecideCapacity() =>
            options.Capacity == 0 ?
                prioritizedChannel ? ChannelConfig.DefaultPriorityChannelCapacity : ChannelConfig.DefaultCapacity
                : options.Capacity;
    }

    private void CreateMqOrderPublisher(IServiceScope serviceScope, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested || _mqConfiguration.DisableMq)
        {
            return;
        }
        var mqConnectionFactory = serviceScope.ServiceProvider.GetRequiredService<IMqConnectionFactory>();
        var mqConnectionProperties = _mapper.Map<MqConnectionProperties>(_mqConfiguration);
        if (mqConnectionProperties != null)
        {
            _mqOrderPublisher = mqConnectionFactory.CreateOrderPublisher(mqConnectionProperties) ?? throw new ArgumentNullException(nameof(_mqOrderPublisher));
            _logger.LogInformation($"{nameof(OBChannelHandlerService)} {nameof(CreateMqOrderPublisher)} Done.");
        }
        else
        {
            _logger.LogError($"Error creating MQ order publisher as MQ connection properties is empty");
            throw new ArgumentNullException(nameof(_mqOrderPublisher));
        }
    }


    public bool TryWrite(PieResponseMessage message, bool? prioritized = false)
    {
        message.IsPriority = prioritized ?? false;
        return prioritized == true ? _priorityChannel.Writer.TryWrite(message) : _channel.Writer.TryWrite(message);
    }

    public async Task WriteAsync(PieResponseMessage message, CancellationToken cancellationToken, bool? prioritized = false)
    {
        message.IsPriority = prioritized ?? false;
        var linkedCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cancellationToken ?? CancellationToken.None).Token;
        if (prioritized == true)
        {
            await _priorityChannel.Writer.WriteAsync(message, linkedCancellationToken);
            return;
        }
        await _channel.Writer.WriteAsync(message, linkedCancellationToken);
    }

    public async ValueTask<bool> WaitToWriteAsync(PieResponseMessage message, CancellationToken cancellationToken, bool? prioritized = false) =>
        prioritized == true ? await WaitToWritePrioritizedMessage(message, cancellationToken) : await WaitToWriteNormalMessage(message, cancellationToken);

    private async Task<bool> WaitToWriteNormalMessage(PieResponseMessage message, CancellationToken cancellationToken)
    {
        message.IsPriority ??= false;
        while (await _channel.Writer.WaitToWriteAsync(cancellationToken) && _cancellationToken?.IsCancellationRequested != true)
        {
            if (_channel.Writer.TryWrite(message))
            {
                return true;
            }
        }
        return false;
    }

    private async Task<bool> WaitToWritePrioritizedMessage(PieResponseMessage message, CancellationToken cancellationToken)
    {
        message.IsPriority ??= true;
        while (await _priorityChannel.Writer.WaitToWriteAsync(cancellationToken) && _cancellationToken?.IsCancellationRequested != true)
        {
            if (_priorityChannel.Writer.TryWrite(message))
            {
                return true;
            }
        }
        return false;
    }

    public async Task HandleMessage(PieResponseMessage pieResponseMessage, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing outbound message: {LogMessage}", pieResponseMessage.LogMessage);

        _pieOrderRepository?.Add(pieResponseMessage, cancellationToken);
        _logger.LogDebug("Persisted PHS outbound response message success");
        await WriteAsync(pieResponseMessage, cancellationToken);
    }

    private async Task DelayTime(CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(_channelConfig.ChannelReadDelayMs, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogExceptionAsWarning(ex);
        }
    }

    private async Task ProcessPrioritizedMessageBatch(IServiceScope scope, CancellationToken cancellationToken)
    {
        const string prioritizedMessage = $"{nameof(OBChannelHandlerService)}.PrioritizedMessage";
        try
        {
            while (++_priorityCounter % (_channelConfig.PriorityChannelPrecedenceCount + 1) != 0 && _priorityChannel.Reader.TryRead(out var message))
            {
                _logger.LogInformation("{MethodName} processing {FileName} for mixNumber: {MixNumber}, oeId: {OeId} as {MessageType}",
                    nameof(ProcessPrioritizedMessageBatch), message?.FileName, message?.MixNumber, message?.OeId, prioritizedMessage);

                await ProcessMessage(message, cancellationToken);
            }
            _priorityCounter = 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{prioritizedMessage} Error processing prioritized message");
        }
    }

    private async Task ProcessNormalMessage(IServiceScope scope, CancellationToken cancellationToken)
    {
        const string normalMessage = $"{nameof(OBChannelHandlerService)}.NormalMessage";
        try
        {
            if (!_channel.Reader.TryRead(out var message))
            {
                return;
            }
            _logger.LogInformation("{MethodName} processing {FileName} for mixNumber: {MixNumber}, oeId: {OeId} as {MessageType}",
                nameof(ProcessNormalMessage), message?.FileName, message?.MixNumber, message?.OeId, normalMessage);
            await ProcessMessage(message, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{normalMessage} Error processing prioritized message");
        }
    }

    /**
    ** This method will not get called from any service and will be used as trigger point for REST interface/reprocessing.
    */
    public async Task ProcessOutboundMessageFromChannel(CancellationToken cancellationToken)
    {
        using var topLevelScope = _serviceScopeFactory.CreateScope();
        await ProcessPrioritizedMessageBatch(topLevelScope, cancellationToken);
        await ProcessNormalMessage(topLevelScope, cancellationToken);
        await DelayTime(cancellationToken);
    }

    private async Task ProcessMessage(PieResponseMessage message, CancellationToken cancellationToken)
    {
        const string methodName = nameof(ProcessMessage);
        _logger.LogInformation("{MethodName} with MixNumber: {MixNumber} and OeId: {OeId}", methodName, message.MixNumber, message.OeId);
        await PublishPieHandlerResponse(message);
    }

    private async Task PublishPieHandlerResponse(PieResponseMessage pieResponseMessage)
    {

        if (null != _mqOrderPublisher)
        {
            await _mqOrderPublisher.PublishFactoryOrder(pieResponseMessage);
        }
        else
        {
            _logger.LogError("Error publishing handling pie response message as mqOrderPublisher is not initialized");
        }
    }
}