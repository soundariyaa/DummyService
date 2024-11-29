namespace PieHandlerService.Core.Models;

public sealed class InstallationTimeInfo
{
    public ConnectionType ConnectionType { get; set; }
    public long EstimatedInstallationTime { get; set; }
}