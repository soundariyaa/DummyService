namespace PieHandlerService.Core.Models;

public class OrderSpecification(
    string mixNumber,
    string oeIdentifier,
    SIIGOrderType orderType)
{
    public string? MixNumber { get; set; } = mixNumber;

    public string? OeIdentifier { get; set; } = oeIdentifier;

    public SIIGOrderType? OrderType { get; set; } = orderType;
}