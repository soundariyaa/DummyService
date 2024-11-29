namespace PieHandlerService.Core.Models;

public enum EcuMemoryType { Unknown, Single, Dual }

public sealed class Ecu
{
    public ushort? EcuAddress { get; set; }
    public string? EcuName { get; set; }
    public EcuMemoryType EcuMemoryType { get; set; } = EcuMemoryType.Unknown;
    public bool EcuReplaced { get; set; }
    public string? HardwarePartNumber { get; set; }
    public string? HardwareSerialNumber { get; set; }
    public int? SecurityAccessGen { get; set; }
    public SoftwareSigningType SoftwareSigningType { get; set; }
    public SoftwareSigningKey? SoftwareSigningKey { get; set; } 
    public IEnumerable<Software> Software { get; set; } = new List<Software>();
}