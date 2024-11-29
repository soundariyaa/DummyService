using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using OE.MQ.IBM.XMS;
using PieHandlerService.Infrastructure.Services.Messaging.Interfaces;
using PieHandlerService.Core.Models;
using Newtonsoft.Json;
using PieHandlerService.Core.Interfaces;

namespace PieHandlerService.Infrastructure.Services.Messaging.Service;

public class MqBroadcastContextListener(ILogger<MqBroadcastContextListener>? logger,
    IIBChannelHandlerService iibChannelHandlerService,
    CancellationToken cancellationToken,
    IMetricsService metricsService,
    MqConnectionProperties mqConnectionProperties) : MqMessageListener(mqConnectionProperties, logger), IMqBroadcastContextListener
{
    private readonly ILogger<MqBroadcastContextListener> _logger = logger ?? NullLogger<MqBroadcastContextListener>.Instance;

    private readonly IMetricsService _metricsService = metricsService ?? throw new ArgumentNullException(nameof(metricsService));
    private readonly CancellationToken _cancellationToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken).Token;
    private readonly IIBChannelHandlerService _iibChannelHandlerService =
        iibChannelHandlerService ?? throw new ArgumentNullException(nameof(iibChannelHandlerService));

    protected override async Task<HandleMessageResult> HandleMessage(MqMessage mqMessage)
    {
        Logger.LogInformation("Message received: {OeCorrelationId}", mqMessage.OeCorrelationId);
        if (!string.IsNullOrEmpty(mqMessage.Text))
        {
            var result = await ProcessBroadcastContextMessage(mqMessage, cancellationToken);
            if (result)
            {
                _metricsService.IncreaseMessageCounters(Constants.Metrics.HandledInboundQueueMetric, Constants.Metrics.SuccessValue);
                return ReturnHandleMessageResult(mqMessage, true);
            }
        }
        _metricsService.IncreaseMessageCounters(Constants.Metrics.HandledInboundQueueMetric, Constants.Metrics.FailureValue);
        Logger.LogWarning("InValid Message");
        return ReturnHandleMessageResult(mqMessage, false);
    }

    private static HandleMessageResult ReturnHandleMessageResult(MqMessage? mqMessage, bool success, bool acknowledge = true)
    {
        if (acknowledge)
        {
            mqMessage?.Acknowledge?.Invoke();
        }
        return new HandleMessageResult(success, acknowledge && mqMessage?.Acknowledge != null);
    }

    private async Task<bool> ProcessBroadcastContextMessage(MqMessage mqMessage, CancellationToken cancellationToken1)
    {
        if (mqMessage == null)
        {
            return false;
        }

        const string normalMessageMq = $"{nameof(MqBroadcastContextListener)}.NormalMessage";

        _logger.LogInformation("{MethodName} with OeCorrelationId {OeCorrelationId} as {MessageType}", nameof(ProcessBroadcastContextMessage), mqMessage.OeCorrelationId, normalMessageMq);
        
        var broadcastContextMessage = !string.IsNullOrEmpty(mqMessage.Text) ? JsonConvert.DeserializeObject<BroadcastContextMessage>(mqMessage.Text) : null;
        if (string.IsNullOrEmpty(broadcastContextMessage?.FileName))
        {
            return false;
        }

        _logger.LogInformation("Parsed Broadcast message from queue successfully {@BroadcastContextMessage}", new
        {
            broadcastContextMessage.MixNumber,
            broadcastContextMessage.OeId,
            broadcastContextMessage.RequestType,
            broadcastContextMessage.ContentHash,
            broadcastContextMessage.OriginHash,
            broadcastContextMessage.FileName,
            broadcastContextMessage.IsPriority
        });
        await _iibChannelHandlerService.PublishMessageToChannel(broadcastContextMessage, cancellationToken);

        return true;
    }

}