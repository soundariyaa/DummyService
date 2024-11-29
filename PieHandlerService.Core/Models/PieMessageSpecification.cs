namespace PieHandlerService.Core.Models;

public class PieMessageSpecification(
    string mixNumber,
    string oeIdentifier,
    SIIGOrderType requestType)
{
    public string? MixNumber { get; set; } = mixNumber;

    public string? OeIdentifier { get; set; } = oeIdentifier;

    public SIIGOrderType? RequestType { get; set; } = requestType;
}