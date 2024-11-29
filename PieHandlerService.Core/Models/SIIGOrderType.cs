using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace PieHandlerService.Core.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SIIGOrderType
{
    [EnumMember(Value = "SIIGOrderPreFlash")]
    SIIGOrderPreFlash,

    [EnumMember(Value = "SIIGOrderEndOfLine")]
    SIIGOrderEndOfLine,

    [EnumMember(Value = "SIIGOrderVehicleKeys")]
    SIIGOrderVehicleKeys
}