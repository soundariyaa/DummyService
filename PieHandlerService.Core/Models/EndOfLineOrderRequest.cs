namespace PieHandlerService.Core.Models;

public sealed class EndOfLineOrderRequest
{
    public IEnumerable<string>? CertificateChain { get; set; }
    public EndOfLine EndOfLine { get; set; } = new EndOfLine();    
    public string MixNumber { get; set; } = string.Empty;
    public VehicleObjectContext VehicleObject { get; set; } =  new VehicleObjectContext();
}