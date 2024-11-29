namespace PieHandlerService.Core.Models;

public sealed class EcuSet
{
    public string? PackageIdentity { get; set; }
    public IEnumerable<Ecu> Ecus { get; set; } = new List<Ecu>();
}