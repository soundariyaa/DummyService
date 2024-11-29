using PieHandlerService.Core.Models;

namespace PieHandlerService.Infrastructure.Services.Storage.Interface;

public interface IStorageHandlerService
{
    Task Handle(BroadcastContextMessage message, CancellationToken cancellationToken);
}