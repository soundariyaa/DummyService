namespace PieHandlerService.Core.Models;

public class VinUniqueSoftware
{
    public string? CarConfig { get; set; }

    public IEnumerable<CertificateData>? Certificates { get; set; }
}