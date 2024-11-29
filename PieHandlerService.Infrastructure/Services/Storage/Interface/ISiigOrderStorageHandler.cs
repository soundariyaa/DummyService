using PieHandlerService.Core.Models;

namespace PieHandlerService.Infrastructure.Services.Storage.Interface;

public interface ISiigOrderStorageHandler
{
    Task<FileStatusDetail> SavePreFlashOrder(PreFlashOrderResponse preFlashOrderResponse, CancellationToken cancellationToken);

    Task<FileStatusDetail> SaveEndOfLineOrder(EndOfLineOrderResponse endOfLineOrderResponse, CancellationToken cancellationToken);

    Task<FileStatusDetail> SaveVehicleCodesOrder(VehicleCodesResponse vehicleCodesResponse, CancellationToken cancellationToken);
}