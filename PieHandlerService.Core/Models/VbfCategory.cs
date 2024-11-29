using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace PieHandlerService.Core.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum VbfCategory
{
    [EnumMember(Value = "STATIC")]
    STATIC,

    [EnumMember(Value = "VIN_UNIQUE")]
    VIN_UNIQUE,

    [EnumMember(Value = "CARCONFIG")]
    CARCONFIG,

    [EnumMember(Value = "ALL")]
    ALL
}