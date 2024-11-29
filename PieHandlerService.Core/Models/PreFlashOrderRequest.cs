namespace PieHandlerService.Core.Models;

public class PreFlashOrderRequest
{
    public IEnumerable<string>? CertificateChain { get; set; }
    public string MixNumber { get; set; } = string.Empty;
    public PreFlash PreFlash { get; set; } = new PreFlash();
    public VehicleObjectContext VehicleObject { get; set; } = new VehicleObjectContext();
}