using OE.MQ.IBM.XMS;
using PieHandlerService.Core.Models;

namespace PieHandlerService.Infrastructure.Services.Messaging.Interfaces;

public interface IMqOrderPublisher : IMqMessageSender {

    Task PublishFactoryOrder(PieResponseMessage pieResponseMessage);
}