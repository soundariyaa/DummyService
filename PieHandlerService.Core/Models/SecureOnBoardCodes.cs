namespace PieHandlerService.Core.Models;

public sealed class SecureOnBoardCodes
{
    public string? ClusterId { get; set; }
    public string? ValueToWrite { get; set; }
    public IEnumerable<EcuCodes> EcuCodes { get; set; } = new List<EcuCodes>();
}