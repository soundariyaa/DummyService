namespace PieHandlerService.Core.Models;

public sealed class Order
{
    public string? OrderHash { get; set; } = string.Empty;
    public EcuSet LoadEcuSet { get; set; } = new EcuSet();
    public string? VehicleCodesSessionKeyId { get; set; } = string.Empty;
    public string? VbfSessionKeyId { get; set; } = string.Empty;
    public string? Version { get; set; } = string.Empty;
    public EncryptionData EncryptionData { get; set; } = new EncryptionData();
    public IEnumerable<VehicleCodeV2> VehicleCodesV2 { get; set; } = new List<VehicleCodeV2>();
    public IEnumerable<SecureOnBoardCodes> SecureOnBoardCodes { get; set; } = new List<SecureOnBoardCodes>();
}