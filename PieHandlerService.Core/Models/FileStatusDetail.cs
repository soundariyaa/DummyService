namespace PieHandlerService.Core.Models;

public sealed class FileStatusDetail(FileStatusType _statusType, string _fileName, long _createdUtcTs, bool _isAvailable)
{

    public FileStatusType StatusType { get; set; } = _statusType;

    public string FileName { get; set; } = _fileName;

    public long CreatedUtcMs { get; set; } = _createdUtcTs;

    public bool IsAvailable { get; set; } = _isAvailable;

    public long? UpdatedUtcMs { get; set; }

    public string? FileHash { get; set; } = string.Empty;

    public string? ContentHash { get; set; } = string.Empty;

    public string? Content { get; set; }
}