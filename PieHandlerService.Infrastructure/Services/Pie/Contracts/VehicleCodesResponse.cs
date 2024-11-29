using PieHandlerService.Core.Models;

namespace PieHandlerService.Infrastructure.Services.Pie.Contracts;

public class VehicleCodesResponse : ResponseBase
{
    public VehicleCodesResponse() { 
        
    }

    public string? PackageIdentity { get; set; } = string.Empty;
    public string? VehicleCodesSessionKeyId { get; set; } = string.Empty;
    public string? VbfSessionKeyId { get; set; } = string.Empty;
    public IEnumerable<VehicleCodeV2> VehicleCodesV2 { get; set; } = new List<VehicleCodeV2>();
    public string? KeyManifest { get; set; } = string.Empty;
    public IEnumerable<SecureOnBoardCodes> SecureOnBoardCodes { get; set; } = new List<SecureOnBoardCodes>();
}