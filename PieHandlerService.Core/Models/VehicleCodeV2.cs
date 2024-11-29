namespace PieHandlerService.Core.Models;

public sealed class VehicleCodeV2
{
    public string? EcuName { get; set; }
    public int SecurityArea { get; set; }
    public string? SecurityId { get; set; }
    public string? UnlockKey { get; set; }
    public string? ValueToWrite { get; set; }
    public string? Key { get; set; }
}