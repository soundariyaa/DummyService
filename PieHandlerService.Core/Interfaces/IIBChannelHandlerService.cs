using Microsoft.Extensions.DependencyInjection;
using PieHandlerService.Core.Models;

namespace PieHandlerService.Core.Interfaces;

public interface IIBChannelHandlerService
{
    bool TryWrite(BroadcastContextMessage message, bool? prioritized = false);
    Task WriteAsync(BroadcastContextMessage message, CancellationToken cancellationToken, bool? prioritized = false);
    ValueTask<bool> WaitToWriteAsync(BroadcastContextMessage message, CancellationToken cancellationToken, bool? prioritized = false);
    Task PublishMessageToChannel(BroadcastContextMessage broadcastMessage, CancellationToken cancellationToken);
    Task ProcessBroadcastMessage(CancellationToken cancellationToken, bool isPriotizedMessage);
    void RegisterMqBroadcastContextListener(IServiceScope serviceScope, CancellationToken cancellationToken);
    void UnRegisterMqBroadcastContextListener();
}