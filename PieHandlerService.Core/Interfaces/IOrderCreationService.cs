using PieHandlerService.Core.Models;

namespace PieHandlerService.Core.Interfaces;

public interface IOrderCreationService
{
    Task<EndOfLineOrderResponse> CreateEndOfLineOrder(string? oeIdentifier, string? mixNumber, EndOfLineContext endOfLineContext ,CancellationToken cancellationToken);

    Task<PreFlashOrderResponse> CreatePreFlashOrder(string? oeIdentifier, string? mixNumber, PreFlashContext preFlashContext, CancellationToken cancellationToken);

    Task<VehicleCodesResponse> CreateVehicleCodes(string? oeIdentifier, string? mixNumber, VehicleObjectContext vehicleObjectContext, CancellationToken cancellationToken);
}