

namespace PieHandlerService.Core.Models;

public sealed class EndOfLineContext
{
    public string EndOfLineHash { get; set; } = string.Empty;

    public string State { get; set; } = string.Empty;

    public string Orderer { get; set; } = string.Empty;

    public string Pno12 { get; set; } = string.Empty;

    public long CreatedUtcMs { get; set; }

    public long ModifiedUtcMs { get; set; }

    public IEnumerable<EcuSoftwareDetail> Ecus { get; set;} = Enumerable.Empty<EcuSoftwareDetail>();

    public VinUniqueSoftware VinUnique { get; set; } = new VinUniqueSoftware(); 
}