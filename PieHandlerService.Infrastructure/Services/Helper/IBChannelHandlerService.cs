using AutoMapper;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using PieHandlerService.Core.Interface;
using PieHandlerService.Core.Models;
using PieHandlerService.Infrastructure.Repositories.Message;
using PieHandlerService.Infrastructure.Services.Storage.Interface;
using System.Threading.Channels;
using PieHandlerService.Core.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using OE.MQ.IBM.XMS;
using PieHandlerService.Infrastructure.Services.Messaging.Interfaces;
using PieHandlerService.Infrastructure.Services.Messaging;
using PieHandlerService.Infrastructure.Extensions;

namespace PieHandlerService.Infrastructure.Services.Helper;

public sealed class IBChannelHandlerService(IHostApplicationLifetime lifetime, IMapper mapper, IOptions<MqConfiguration> ibmMqConfigurationOptions, IOptions<ChannelConfig> channelConfig, ILogger<IBChannelHandlerService> logger, 
    IServiceScopeFactory serviceScopeFactory) : BackGroundWorkerService(lifetime), IIBChannelHandlerService
{

    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));

    private readonly ILogger<IBChannelHandlerService>? _logger = logger ?? NullLogger<IBChannelHandlerService>.Instance;

    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    private readonly MqConfiguration _mqConfiguration = ibmMqConfigurationOptions.Value ?? throw new ArgumentNullException(nameof(ibmMqConfigurationOptions));
    private IStorageHandlerService? _storageHandlerService;

    private readonly ChannelConfig _channelConfig = channelConfig?.Value ?? new();
    private IBroadcastMetadataRepository? _broadcastMetadataRepo;

    private IMqBroadcastContextListener? _mqBroadcastMessageListener;

    private readonly Channel<BroadcastContextMessage> _channel = CreateMessageChannel(channelConfig?.Value ?? new ChannelConfig(), false);
    private readonly Channel<BroadcastContextMessage> _priorityChannel = CreateMessageChannel(channelConfig?.Value ?? new ChannelConfig(), true);

    private int _priorityCounter;

    private CancellationToken _cancellationToken;

    private const string MqConsumerKey = $"{nameof(PieHandlerService)}.{nameof(IBChannelHandlerService)}";


    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _logger?.LogInformation($"{nameof(IBChannelHandlerService)} started: ");
        using var topLevelScope = _serviceScopeFactory.CreateScope();
        RegisterMqBroadcastContextListener(topLevelScope, cancellationToken);
        InjectDependencies(topLevelScope);
        return base.StartAsync(cancellationToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger?.LogInformation($"{nameof(IBChannelHandlerService)} stopped:");
        UnRegisterMqBroadcastContextListener();
        return base.StopAsync(cancellationToken);
    }

    private void InjectDependencies(IServiceScope serviceScope) {
        _broadcastMetadataRepo = serviceScope.ServiceProvider.GetRequiredService<IBroadcastMetadataRepository>() ?? throw new ArgumentNullException(nameof(_broadcastMetadataRepo));
        _storageHandlerService = serviceScope.ServiceProvider.GetRequiredService<IStorageHandlerService>() ?? throw new ArgumentNullException(nameof(_storageHandlerService));
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger?.LogInformation($"{nameof(IBChannelHandlerService)} {nameof(ExecuteAsync)} started.");

        _cancellationToken = cancellationToken;
        await AwaitApplicationStarted(cancellationToken);

        while (!cancellationToken.IsCancellationRequested)
        {;
            await ProcessPrioritizedMessageBatch(cancellationToken);
            await ProcessNormalMessage(cancellationToken);
            await DelayTime(cancellationToken);
        }

        _logger?.LogInformation($"{nameof(IBChannelHandlerService)}  ExecuteAsync Terminating.");
    }

    private static Channel<BroadcastContextMessage> CreateMessageChannel(ChannelConfig options, bool prioritizedChannel)
    {
        return options.Capacity < 0
            ? Channel.CreateUnbounded<BroadcastContextMessage>()
            : Channel.CreateBounded<BroadcastContextMessage>(
                new BoundedChannelOptions(DecideCapacity())
                {
                    FullMode = options.FullMode,
                    SingleReader = true,
                    SingleWriter = true
                });

        int DecideCapacity() =>
            options.Capacity == 0 ?
                prioritizedChannel ? ChannelConfig.DefaultPriorityChannelCapacity : ChannelConfig.DefaultCapacity
                : options.Capacity;
    }

    public bool TryWrite(BroadcastContextMessage message, bool? prioritized = false)
    {
        message.IsPriority = prioritized ?? false;
        return prioritized == true ? _priorityChannel.Writer.TryWrite(message) : _channel.Writer.TryWrite(message);
    }

    public async Task WriteAsync(BroadcastContextMessage message, CancellationToken cancellationToken, bool? prioritized = false)
    {
        message.IsPriority = prioritized ?? false;
        var linkedCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cancellationToken).Token;
        if (prioritized == true)
        {
            await _priorityChannel.Writer.WriteAsync(message, linkedCancellationToken);
            return;
        }
        var result =  _channel.Writer.WriteAsync(message, linkedCancellationToken);
        _logger?.LogInformation("Written to the inbound channel {IsCompletedSuccessfully}", result.IsCompletedSuccessfully);
    }

    public async ValueTask<bool> WaitToWriteAsync(BroadcastContextMessage message, CancellationToken cancellationToken, bool? prioritized = false) =>
        prioritized == true ? await WaitToWritePrioritizedMessage(message, cancellationToken) : await WaitToWriteNormalMessage(message, cancellationToken);

    private async Task<bool> WaitToWriteNormalMessage(BroadcastContextMessage message, CancellationToken cancellationToken)
    {
        message.IsPriority ??= false;
        while (await _channel.Writer.WaitToWriteAsync(cancellationToken) && _cancellationToken.IsCancellationRequested != true)
        {
            if (_channel.Writer.TryWrite(message))
            {
                return true;
            }
        }
        return false;
    }

    private async Task<bool> WaitToWritePrioritizedMessage(BroadcastContextMessage message, CancellationToken cancellationToken)
    {
        message.IsPriority ??= true;
        while (await _priorityChannel.Writer.WaitToWriteAsync(cancellationToken) && _cancellationToken.IsCancellationRequested != true)
        {
            if (_priorityChannel.Writer.TryWrite(message))
            {
                return true;
            }
        }
        return false;
    }

    public async Task PublishMessageToChannel(BroadcastContextMessage broadcastMessage, CancellationToken cancellationToken = default)
    {
        var broadcastSpec = new BroadcastMessageSpecification(broadcastMessage.MixNumber, broadcastMessage.OeId,broadcastMessage.RequestType);
        if (null != _broadcastMetadataRepo)
        {
            var existingRecords = await _broadcastMetadataRepo.FetchAll(BroadcastMetadataRepository.GenerateExpressionFilterforBroadcast(broadcastSpec));

            if (existingRecords.Any())
            {
                _logger?.LogError("Broadcast message has been processed already for the MixNumber {MixNumber} OeId {OeId}",
                    broadcastMessage?.MixNumber,broadcastMessage?.OeId);
                return;
            }
            var result = _broadcastMetadataRepo.Add(broadcastMessage, cancellationToken);
            _logger?.LogInformation("Persisted broadcast context {IsCompletedSuccessfully}", result.IsCompletedSuccessfully);
        }
        else
        {
            _logger?.LogError("Error persisting broadcast message due to broadcast repo and problemDetailManager are uninitialized");
        }
        await WriteAsync(broadcastMessage,cancellationToken,broadcastMessage.IsPriority); 
    }


    private async Task DelayTime(CancellationToken cancellationToken)
    {
        try { await Task.Delay(_channelConfig.ChannelReadDelayMs, cancellationToken); }
        catch (Exception ex) { _logger.LogExceptionAsWarning(ex); }
    }

    private async Task ProcessPrioritizedMessageBatch(CancellationToken cancellationToken)
    {
        const string prioritizedMessage = $"{nameof(IBChannelHandlerService)}.PrioritizedMessage";
        try
        {
            while (++_priorityCounter % (_channelConfig.PriorityChannelPrecedenceCount + 1) != 0 && _priorityChannel.Reader.TryRead(out var message))
            {
                _logger?.LogInformation("{MethodName} processing {RequestType} for mixNumber: {MixNumber}, oeId: {OeId}  as {MessageType}",
                    nameof(ProcessNormalMessage), message?.RequestType, message?.MixNumber, message?.OeId, prioritizedMessage);
                await ProcessMessage( message, cancellationToken);
            }
            _priorityCounter = 0;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "{MethodName} Error processing {MessageType}", nameof(ProcessPrioritizedMessageBatch), prioritizedMessage);
        }
    }

    private async Task ProcessMessage(BroadcastContextMessage message, CancellationToken cancellationToken)
    {
        if (null != _storageHandlerService) { await _storageHandlerService.Handle(message, cancellationToken); }
        else { _logger?.LogError("Error processing inbound message due storage handler service not initialized."); }
    }

    private async Task ProcessNormalMessage(CancellationToken cancellationToken)
    {
        const string normalMessage = $"{nameof(IBChannelHandlerService)}.NormalMessage";
        try
        {
            if (!_channel.Reader.TryRead(out var message))
            {
                return;
            }
            _logger?.LogInformation("{MethodName} processing {RequestType} for mixNumber: {MixNumber}, oeId: {OeId}  as {MessageType}",
                nameof(ProcessNormalMessage), message?.RequestType, message?.MixNumber, message?.OeId, normalMessage);
            await ProcessMessage(message, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "{MethodName} Error processing {MessageType}", nameof(ProcessNormalMessage), normalMessage);
        }
    }

    public void RegisterMqBroadcastContextListener(IServiceScope serviceScope, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested || _mqConfiguration.DisableMq)
        {
            return;
        }
        var mqConnectionFactory = serviceScope.ServiceProvider.GetRequiredService<IMqConnectionFactory>();
        var mqConnectionProperties = _mapper.Map<MqConnectionProperties>(_mqConfiguration);
        try
        {
            if (mqConnectionProperties != null)
            {
                _mqBroadcastMessageListener = mqConnectionFactory.CreateBroadcastContextListener(mqConnectionProperties, cancellationToken);
                _mqBroadcastMessageListener.RegisterConsumer(
                    MqConsumerKey, new MqDestination { Type = DestinationType.Queue, Name = _mqConfiguration.QueueName! }, null);
                _mqBroadcastMessageListener.Start(cancellationToken);
            }
            else
            {
                _logger?.LogError($"Error registering Broadcast listener to the inbound message queue");
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error registering Broadcast listener to the inbound message queue {ErrorMessage}", ex.Message);
        }
        _logger?.LogInformation($"{nameof(IBChannelHandlerService)} {nameof(RegisterMqBroadcastContextListener)} Done.");
    }

    public void UnRegisterMqBroadcastContextListener()
    {
        try {
            _mqBroadcastMessageListener?.Stop();
            _mqBroadcastMessageListener?.UnRegisterConsumer(MqConsumerKey);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "{MethodName} Error unregistering Broadcast listener to the inbound message queue {ErrorMessage}",
                nameof(UnRegisterMqBroadcastContextListener), ex.Message);
        }
    }

    /**
    ** This method is not called from any services. This will be used as a trigger point in case if we want to trigger reprocessing of already processed message
    */
    public async Task ProcessBroadcastMessage(CancellationToken cancellationToken, bool isPrioritizedMessage)
    {
        await ProcessPrioritizedMessageBatch(cancellationToken);
        await ProcessNormalMessage(cancellationToken);
        await DelayTime(cancellationToken);
    }
}