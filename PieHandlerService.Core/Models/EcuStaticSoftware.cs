namespace PieHandlerService.Core.Models;

public sealed class EcuStaticSoftware
{
    public string PartNumber { get; set; } = string.Empty;
    public string? State { get; set; }
    public long ModifiedUtcMs { get; set; }
}