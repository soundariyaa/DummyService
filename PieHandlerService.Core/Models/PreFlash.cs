namespace PieHandlerService.Core.Models;

public class PreFlash
{
    public IEnumerable<EcuSoftwareDetail>? Ecus { get; set; } = Enumerable.Empty<EcuSoftwareDetail>();
}