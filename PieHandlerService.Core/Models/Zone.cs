using System.Runtime.Serialization;
using System.Text.Json.Serialization;


namespace PieHandlerService.Core.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Zone
{
    [EnumMember(Value = "BatteryEOL")]
    BatteryEOL,

    [EnumMember(Value = "PreFlash")]
    PreFlash,

    [EnumMember(Value = "EOL")]
    EOL,

    [EnumMember(Value = "BrakeFill")]
    BrakeFill,

    [EnumMember(Value = "VISP")]
    VISP,

    [EnumMember(Value = "FHC")]
    FHC,

    [EnumMember(Value = "FAS")]
    FAS,

    [EnumMember(Value = "CODES")]
    CODES
}