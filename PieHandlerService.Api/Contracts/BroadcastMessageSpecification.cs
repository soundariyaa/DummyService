using MediatR;
using PieHandlerService.Core.Models;


namespace PieHandlerService.Api.Contracts;

public class BroadcastMessageSpecification : IRequest<BroadcastMessageResponse>
{
    public string? MixNumber { get; set; }

    public string? OeIdentifier { get; set; }

    public RequestType? RequestType { get; set; }

}