using MediatR;

namespace PieHandlerService.Api.Contracts;

public sealed class AdminRequest : IRequest<AdminResponse>
{
    public bool RegisterQueue { get; set; }
}