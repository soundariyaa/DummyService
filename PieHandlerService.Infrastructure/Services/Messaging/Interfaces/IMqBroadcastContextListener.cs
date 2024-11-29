using OE.MQ.IBM.XMS;

namespace PieHandlerService.Infrastructure.Services.Messaging.Interfaces;

public interface IMqBroadcastContextListener : IMqMessageListener;

internal interface IMqConnectionFactory
{
    public IMqBroadcastContextListener CreateBroadcastContextListener(MqConnectionProperties mqConnectionProperties, CancellationToken cancellationToken);

    public IMqOrderPublisher CreateOrderPublisher(MqConnectionProperties mqConnectionProperties);
}