using Newtonsoft.Json;

namespace PieHandlerService.Core.Models;

public sealed class SlaveNode
{
    // Id is used to identify which SlaveNode the serial / part-number belongs to
    [JsonIgnore]
    public int Id { get; set; }
    public string? HardwarePartNumber { get; set; }
    public string? HardwareSerialNumber { get; set; }
}