namespace PieHandlerService.Infrastructure.Services.Pie.Contracts;

public class EcuSoftware
{
    public string? EcuName { get; set; } = string.Empty;
    public string? AuthenticationTemplate { get; set; } = string.Empty;
    public IEnumerable<string>? Software { get; set; }
}