namespace PieHandlerService.Core.Models;

public sealed class Software
{
    public ushort? CallAddress { get; set; }
    public uint? DataFileChecksum { get; set; }
    public ushort? DataFormatIdentifier { get; set; }
    public string? DiagnosticPartNumber { get; set; }
    public IEnumerable<EraseData> EraseData { get; set; } = new List<EraseData>();
    public IEnumerable<InstallationTimeInfo> InstallationTimeInfo { get; set; } = new List<InstallationTimeInfo>();
    public string? SoftwarePartNumber { get; set; }
    public string? SoftwarePartSignatureDev { get; set; }
    public string? SoftwarePartSignatureProd { get; set; }
    public long? SoftwarePartSize { get; set; }
    public string? SoftwarePartType { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string? FileEncryptionKey { get; set; }
    public string? FileSignature { get; set; }
    public EncryptionType EncryptionType { get; set; }
}