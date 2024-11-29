using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace PieHandlerService.Core.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RequestType
{
    [EnumMember(Value = "VehicleObject")]
    VehicleObject,

    [EnumMember(Value = "PreFlash")]
    PreFlash,

    [EnumMember(Value = "EndOfLine")]
    EndOfLine
}