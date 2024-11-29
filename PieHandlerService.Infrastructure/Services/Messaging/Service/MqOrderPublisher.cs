using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using OE.MQ.IBM.XMS;
using PieHandlerService.Core.Interfaces;
using PieHandlerService.Core.Models;
using PieHandlerService.Infrastructure.Extensions;
using PieHandlerService.Infrastructure.Services.Messaging.Interfaces;


namespace PieHandlerService.Infrastructure.Services.Messaging.Service;

public class MqOrderPublisher(ILogger<MqOrderPublisher>? logger, IMetricsService metricsService, MqConnectionProperties mqConnectionProperties) : MqConnectedMessageSender(mqConnectionProperties), IMqOrderPublisher
{
    private readonly IMetricsService _metricsService = metricsService ?? throw new ArgumentNullException(nameof(metricsService));

    private ILogger<MqOrderPublisher> _logger = logger ?? NullLogger<MqOrderPublisher>.Instance;

    public async Task PublishFactoryOrder(PieResponseMessage pieResponseMessage)
    {
        var output = JsonConvert.SerializeObject(pieResponseMessage);
        var queueName = Environment.GetEnvironmentVariable(Constants.EnvironmentVariables.OrderPublishQueue) ?? string.Empty;
        var isPublished = false;
        try
        {
            var mqMessage = new MqMessage();
            mqMessage.Text = output;
            mqMessage.OeCorrelationId = Guid.NewGuid();
            mqMessage.MessageId = pieResponseMessage.MixNumber;
            mqMessage.Priority = 0;
            SendMessage(new MqDestination { Name = queueName, Type = DestinationType.Queue }, mqMessage);
            isPublished = true;
            await DelayTime();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing message to the outbound queue {QueueName} " +
                             "for MixNumber: {MixNumber}, OeId: {OeId} and fileName: {FileName}",
                queueName, pieResponseMessage.MixNumber, pieResponseMessage.OeId, pieResponseMessage.FileName);

        }
        finally
        {
            _logger.LogInformation($"{nameof(MqOrderPublisher)} handling message");
            _metricsService.IncreaseMessageCounters(Constants.Metrics.HandledInboundQueueMetric, isPublished ? Constants.Metrics.SuccessValue
                : Constants.Metrics.FailureValue);
        }
           
        return;
    }


    private async Task DelayTime()
    {
        try { await Task.Delay(5); }
        catch (Exception ex) { _logger.LogExceptionAsWarning(ex); }
    }
}