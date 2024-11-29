namespace PieHandlerService.Core.Models;

public class OrderDetails
{
    public string? MixNumber { get; set; }

    public string? OeIdentifier { get; set; }

    public Order? PreFlashOrder { get; set; }

    public Order? EndOfLineOrder { get; set; }

    public Order? VehicleCodesOrder { get; set; }
}