namespace PieHandlerService.Core.Models;

public sealed class EndOfLine
{
    public string Orderer { get; set; } = string.Empty;
    public string Pno12 { get; set; } = string.Empty;
    public IEnumerable<EcuSoftwareDetail> Ecus { get; set; } = Enumerable.Empty<EcuSoftwareDetail>();
    public VinUniqueSoftware VinUnique { get; set; } = new VinUniqueSoftware();
}