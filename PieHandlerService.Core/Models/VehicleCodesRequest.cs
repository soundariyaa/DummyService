

namespace PieHandlerService.Core.Models;

public sealed class VehicleCodesRequest 
{
    public IEnumerable<string>? CertificateChain { get; set; }
    public string? MixNumber { get; set; }
    public VehicleObjectContext VehicleObject { get; set; } = new VehicleObjectContext();
}