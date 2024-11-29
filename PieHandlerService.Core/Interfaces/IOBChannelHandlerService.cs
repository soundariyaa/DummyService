using PieHandlerService.Core.Models;


namespace PieHandlerService.Core.Interfaces;

public interface IOBChannelHandlerService
{
    bool TryWrite(PieResponseMessage message, bool? prioritized = false);
    Task WriteAsync(PieResponseMessage message, CancellationToken cancellationToken, bool? prioritized = false);
    ValueTask<bool> WaitToWriteAsync(PieResponseMessage message, CancellationToken cancellationToken, bool? prioritized = false);
    Task HandleMessage(PieResponseMessage pieResponseMessage, CancellationToken cancellationToken);
}