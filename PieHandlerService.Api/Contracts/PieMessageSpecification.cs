using MediatR;
using PieHandlerService.Core.Models;

namespace PieHandlerService.Api.Contracts;

public class PieMessageSpecification : IRequest<PieOrderMessageResponse>
{
    public string? OeIdentifier { get; set; }
    public string? MixNumber { get; set; }
    public SIIGOrderType? RequestType { get; set; }
}