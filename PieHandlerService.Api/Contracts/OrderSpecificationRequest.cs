using MediatR;
using PieHandlerService.Core.Models;

namespace PieHandlerService.Api.Contracts;

public class OrderSpecificationRequest : IRequest<OrderSpecificationResponse>
{
    public string? MixNumber { get; set; }

    public string? OeIdentifier { get; set; }

    public SIIGOrderType? OrderType { get; set; }

}