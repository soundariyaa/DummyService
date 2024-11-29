using MediatR;
using PieHandlerService.Core.Models;

namespace PieHandlerService.Api.Contracts;

public class PreFlashOrderRequest : IRequest<PreFlashOrderResponse>
{
    public string? MixNumber { get; set; }

    public string? OeId { get; set; }

    public long CreatedUtcMs { get; set; }

    public string? OriginHash { get; set; }

    public bool? IsPriority { get; set; }

    public bool? ForcedOverrideCompletionRule { get; set; }

    public required PreFlashContext PreFlashContext { get; set; } = new PreFlashContext();

}