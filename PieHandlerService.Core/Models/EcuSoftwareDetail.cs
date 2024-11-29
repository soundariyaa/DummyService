namespace PieHandlerService.Core.Models;

public sealed class EcuSoftwareDetail
{
    public string? EcuName { get; set; }

    public string? AuthenticationTemplate { get; set; }

    public IEnumerable<EcuStaticSoftware>? Software { get; set; }

}