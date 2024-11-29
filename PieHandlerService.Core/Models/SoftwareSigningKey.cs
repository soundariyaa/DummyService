namespace PieHandlerService.Core.Models;

public sealed class SoftwareSigningKey
{
    public string? PublicKeyModulus { get; set; }
    public string? PublicKeyExponent { get; set; }
    public string? PublicKeyChecksum { get; set; }
    public string? PublicKey { get; set; }
    public bool IsEncrypted { get; set; }
}