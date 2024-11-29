using PieHandlerService.Core.Models;

namespace PieHandlerService.Infrastructure.Services.Storage.Interface;

public interface IBroadcastContextStorageHandler
{
    Task<EndOfLineOrderFile> FetchBroadcastContextForEndOfLine(BroadcastContextMessage broadcastContextMessage,CancellationToken cancellationToken);

    Task<PreFlashOrderFile> FetchBroadcastContextForPreFlash(BroadcastContextMessage broadcastContextMessage, CancellationToken cancellationToken);

    Task<VehicleCodeFile> FetchBroadcastContextForVehicleCodes(BroadcastContextMessage broadcastContextMessage, CancellationToken cancellationToken);
}