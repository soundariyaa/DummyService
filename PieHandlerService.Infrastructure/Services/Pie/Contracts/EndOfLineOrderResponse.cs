using PieHandlerService.Core.Models;

namespace PieHandlerService.Infrastructure.Services.Pie.Contracts;

public class EndOfLineOrderResponse : ResponseBase
{
    public EndOfLineOrderResponse() { }
    public Order Order { get; set; } = new Order();
    public string? PieKeyManifest { get; set; }
    public string? StorageLocation { get; set; } = string.Empty;
}