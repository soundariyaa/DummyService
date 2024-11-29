using PieHandlerService.Core.Models;

namespace PieHandlerService.Infrastructure.Services.Pie.Contracts;

public class EndOfLineOrderRequest
{
    public List<string>? CertificateChain { get; set; }
    public string? MixNumber { get; set; }
    public EndOfLineData? EndOfLine { get; set; }
    public VehicleObjectContext? VehicleObject { get; set; }
    public VinUniqueSoftware VinUnique { get; set; } = new VinUniqueSoftware();
}

public sealed class EndOfLineData
{
    public string? Orderer { get; set; }
    public string? Pno12 { get; set; }
    public IEnumerable<EcuSoftware>? Ecus { get; set; }
}