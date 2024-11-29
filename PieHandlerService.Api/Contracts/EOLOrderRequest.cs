using MediatR;
using PieHandlerService.Core.Models;

namespace PieHandlerService.Api.Contracts;

public sealed class EOLOrderRequest : IRequest<EOLOrderResponse>
{
    public string MixNumber { get; set; } = string.Empty;

    public string OeId { get; set; } = string.Empty;

    public long CreatedUtcMs { get; set; }

    public string OriginHash { get; set; } = string.Empty;

    public bool? IsPriority { get; set; }

    public bool? ForcedOverrideCompletionRule { get; set; }

    public EndOfLineContext EndOfLineContext { get; set; } = new EndOfLineContext();

}