namespace PieHandlerService.Core.Models;

public sealed class VehicleCodeFile
{
    public string MixNumber { get; set; } = string.Empty;

    public string OeId { get; set; } = string.Empty;

    public long CreatedUTCMs { get; set; }

    public string OriginHash { get; set; } = string.Empty;

    public bool? IsPriority { get; set; }

    public bool? ForcedOverrideCompletionRule { get; set; }

    public VehicleObjectContext VehicleObjectContext { get; set; } = new VehicleObjectContext();
}