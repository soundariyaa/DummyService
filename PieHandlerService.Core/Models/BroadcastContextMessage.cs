using System.Text.Json.Serialization;

namespace PieHandlerService.Core.Models;

public sealed class BroadcastContextMessage
{
    public BroadcastContextMessage() { 
    }

    public string Id { get; set; } = string.Empty;

    public required string OeId { get; set; } = string.Empty;

    public required string MixNumber { get; set; } = string.Empty;

    public long? CreatedUtcMs { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        
    public long? ModifiedUtcMs { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required RequestType RequestType { get; set; } = RequestType.EndOfLine;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public OrderStatus Status { get; set; } = OrderStatus.Available;
        
    public required string FileName { get; set; } = string.Empty;

    public required string? ContentHash { get; set; }

    public required string? OriginHash { get; set; }

    public required string? Provider { get; set; } = string.Empty;

    public bool? Independent { get; set; } = true;

    public bool? IsPriority { get; set; } = false;
}
