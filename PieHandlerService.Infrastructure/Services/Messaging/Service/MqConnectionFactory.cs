using Microsoft.Extensions.Logging;
using OE.MQ.IBM.XMS;
using PieHandlerService.Core.Interfaces;
using PieHandlerService.Infrastructure.Services.Messaging.Interfaces;

namespace PieHandlerService.Infrastructure.Services.Messaging.Service;

public class MqConnectionFactory(IIBChannelHandlerService iibChannelHandlerService, IMetricsService metricsService, ILoggerFactory loggerFactory) : IMqConnectionFactory
{

    private readonly IIBChannelHandlerService _iibChannelHandlerService =
        iibChannelHandlerService ?? throw new ArgumentNullException(nameof(iibChannelHandlerService));

    private readonly IMetricsService _metricsService =
        metricsService ?? throw new ArgumentNullException(nameof(metricsService));

    private readonly ILoggerFactory _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));

    public IMqBroadcastContextListener CreateBroadcastContextListener(MqConnectionProperties mqConnectionProperties, CancellationToken cancellationToken)
    {
        return new MqBroadcastContextListener(
            _loggerFactory.CreateLogger<MqBroadcastContextListener>(), _iibChannelHandlerService, cancellationToken, _metricsService,  mqConnectionProperties);
    }

    public IMqOrderPublisher CreateOrderPublisher(MqConnectionProperties mqConnectionProperties) {
        return new MqOrderPublisher(
            _loggerFactory.CreateLogger<MqOrderPublisher>(), _metricsService, mqConnectionProperties);
    }
}