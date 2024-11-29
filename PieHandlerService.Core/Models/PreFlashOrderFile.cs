namespace PieHandlerService.Core.Models;

public sealed class PreFlashOrderFile
{
    public string MixNumber { get; set; } = string.Empty;

    public string OeId { get; set; } = string.Empty;

    public long CreatedUTCMs { get; set; } 

    public string OriginHash { get; set; } = string.Empty;

    public bool? IsPriority { get; set; }

    public bool? ForcedOverrideCompletionRule { get; set; }

    public PreFlashContext PreFlashContext { get; set; } = new PreFlashContext();
}