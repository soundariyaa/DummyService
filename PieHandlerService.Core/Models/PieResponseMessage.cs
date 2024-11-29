using System.Text.Json.Serialization;

namespace PieHandlerService.Core.Models;

[Serializable]
public sealed class PieResponseMessage
{
    public string? Id { get; set; } = string.Empty;
    public required string OeId { get; set; } = string.Empty;
    public required string MixNumber { get; set; } = string.Empty;
    public long? CreatedUtcMs { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    public long? ModifiedUtcMs { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    public required string RequestType { get; set; } = string.Empty;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required OrderStatus Status { get; set; } = OrderStatus.Pending;

    public string? ContentHash { get; set; }
    public required string? OriginHash { get; set; }
    public required string? FileName { get; set; }
    public bool? IsPriority { get; set; } = false;

    public string? Provider { get; set; } = Constants.Settings.NamePieHandlerService;

    [Obsolete("Use structured logging instead", error: false)]
    public string LogMessage => $"PieResponseMessage Content:(" + nameof(OeId) + "=" + OeId + "|" +
                                nameof(MixNumber) + "=" + MixNumber + "|" +
                                nameof(RequestType) + "=" + RequestType + "|" +
                                nameof(ContentHash) + "=" + ContentHash + "|" +
                                nameof(OriginHash) + "=" + OriginHash + "|" +
                                nameof(FileName) + "=" + FileName + "|" +
                                nameof(IsPriority) + "=" + IsPriority + ")";
}