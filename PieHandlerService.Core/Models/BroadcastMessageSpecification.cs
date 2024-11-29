namespace PieHandlerService.Core.Models;

public class BroadcastMessageSpecification
{
    public string MixNumber { get; set; } = string.Empty;

    public string OeIdentifier { get; set; } = string.Empty;

    public RequestType? RequestType { get; set; } 

    public OrderStatus? Status { get; set; }

    public bool Independent { get; set; } = true;


    public BroadcastMessageSpecification(
        string mixNumber,
        RequestType requestType
    )
    {
        MixNumber = mixNumber;
        RequestType = requestType;
    }

    public BroadcastMessageSpecification(
        string mixNumber,
        string oeIdentifier,
        RequestType requestType
    )
    {
        MixNumber = mixNumber;
        OeIdentifier = oeIdentifier;
        RequestType = requestType;
    }

    public BroadcastMessageSpecification(
        string mixNumber,
        string oeIdentifier,
        RequestType requestType,
        OrderStatus? status
    )
    {
        MixNumber = mixNumber;
        OeIdentifier = oeIdentifier;
        RequestType = requestType;
        Status = status;
    }

    public BroadcastMessageSpecification(
    string mixNumber,
    string oeIdentifier,
    RequestType requestType,
    bool independent
)
    {
        MixNumber = mixNumber;
        OeIdentifier = oeIdentifier;
        RequestType = requestType;
        Independent = independent;
    }
}