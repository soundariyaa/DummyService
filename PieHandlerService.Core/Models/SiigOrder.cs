
namespace PieHandlerService.Core.Models;

public class SiigOrder 
{
    public string? Id { get; set; }

    public string? MixNumber { get; set; }

    public string? OeIdentifier { get; set; }

    public string? OrderResponse { get; set; }

    public SIIGOrderType? OrderType { get; set; }

    public OrderStatus? OrderStatus { get; set; }

    public long CreatedUtcTs { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    public long ModifiedUtcTs { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

}