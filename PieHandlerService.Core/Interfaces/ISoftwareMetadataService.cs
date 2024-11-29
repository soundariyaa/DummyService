using PieHandlerService.Core.Models;

namespace PieHandlerService.Core.Interfaces;

public interface ISoftwareMetadataService
{
    Task<EndOfLineOrderResponse> FetchEcuEndOfLineOrder(HttpClient httpClient, EndOfLineOrderRequest endOfLineOrderRequest);

    Task<PreFlashOrderResponse> FetchEcuPreFlashOrder(HttpClient httpClient, PreFlashOrderRequest preFlashOrderRequest);

    Task<VehicleCodesResponse> FetchFactoryVehicleCodes(HttpClient httpClient, VehicleCodesRequest vehicleCodesRequest);
}