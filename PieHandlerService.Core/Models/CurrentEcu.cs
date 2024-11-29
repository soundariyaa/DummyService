namespace PieHandlerService.Core.Models;

public sealed class CurrentEcu
{
    public string? ApplDiagnosticPartNumber { get; set; }
    public string? ApplDiagnosticPartNumberDefault { get; set; }
    public string? AssemblyNumber { get; set; }
    public string EcuAddress { get; set; } = string.Empty;
    public string EcuName { get; set; } = string.Empty;
    public EcuStatus EcuStatus { get; set; } = EcuStatus.E;
    public string? HardwarePartNumber { get; set; }
    public string? HardwareSerialNumber { get; set; }
    public string? Version { get; set; }
    public int? SecurityAccessGen { get; set; }
    public List<Software> SoftwarePartNumbers { get; set; } = new List<Software>();
    public List<SlaveNode> SlaveNodes { get; set; } = new List<SlaveNode>();

}