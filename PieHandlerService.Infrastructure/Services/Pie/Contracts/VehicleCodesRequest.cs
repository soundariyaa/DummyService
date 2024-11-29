using PieHandlerService.Core.Models;


namespace PieHandlerService.Infrastructure.Services.Pie.Contracts;

public class VehicleCodesRequest
{
    public List<string>? CertificateChain { get; set; }
    public string? MixNumber { get; set; }
    public VehicleObjectContext VehicleObject { get; set; } = new VehicleObjectContext();
}