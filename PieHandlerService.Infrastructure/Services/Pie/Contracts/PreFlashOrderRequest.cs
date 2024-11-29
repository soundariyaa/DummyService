using PieHandlerService.Core.Models;

namespace PieHandlerService.Infrastructure.Services.Pie.Contracts;

public class PreFlashOrderRequest
{
    public List<string>? CertificateChain { get; set; }
    public string MixNumber { get; set; } = string.Empty;
    public PreFlashData PreFlash { get; set; } = new PreFlashData();
    public VehicleObjectContext VehicleObject { get; set; } = new VehicleObjectContext();
}

public sealed class PreFlashData
{
    public IEnumerable<EcuSoftware>? Ecus { get; set; }
}