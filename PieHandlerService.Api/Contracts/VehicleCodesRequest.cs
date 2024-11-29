using MediatR;
using PieHandlerService.Core.Models;

namespace PieHandlerService.Api.Contracts;

public sealed class VehicleCodesRequest : IRequest<VehicleCodesResponse>
{
    public required string MixNumber { get; set; } = string.Empty;

    public required string OeId { get; set; } = string.Empty;

    public long CreatedUtcMs { get; set; }

    public string? OriginHash { get; set; }

    public bool? IsPriority { get; set; }

    public bool? ForcedOverrideCompletionRule { get; set; }

    public required VehicleObjectContext VehicleObjectContext { get; set; } = new VehicleObjectContext();
}