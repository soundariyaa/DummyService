namespace PieHandlerService.Core.Models;

public sealed class VehicleCode
{
    public string? SecurityId { get; set; }
    public string? Value { get; set; }
    public string? EcuName { get; set; }
    public string? SessionKeyId { get; set; }
}