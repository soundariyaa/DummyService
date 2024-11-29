namespace PieHandlerService.Core.Models;

public sealed class VehicleObjectContext
{
    public Vehicle Vehicle { get; set; } = new();
    public string? ConfigParamData { get; set; }
    public string? PSpec { get; set; }
    public string? CpvList { get; set; }
    public VdnList VdnList { get; set; } = new();
    public IEnumerable<VehicleCode> VehicleCodes { get; set; } = new List<VehicleCode>();
    public IEnumerable<VehicleCodeV2> VehicleCodesV2 { get; set; } = new List<VehicleCodeV2>();
    public List<CurrentEcu> CurrentEcus { get; set; } = new();
    public string? VehicleObjectHash { get; set; }
    public long ModifiedUtcMs { get; set; }
    public long CreatedUtcMs { get; set; }
}