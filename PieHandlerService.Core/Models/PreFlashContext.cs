namespace PieHandlerService.Core.Models;

public sealed class PreFlashContext
{
    public string? PreFlashHash { get; set; }

    public string? State { get; set; }

    public long ModifiedUtcMs { get; set; }

    public long CreatedUtcMs { get; set; }

    public IEnumerable<EcuSoftwareDetail>? Ecus { get; set; }

}