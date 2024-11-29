using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace PieHandlerService.Core.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum FileStatusType
{
    [EnumMember(Value = "CREATED")]
    CREATED,

    [EnumMember(Value = "UPDATED")]
    UPDATED,

    [EnumMember(Value = "DELETED")]
    DELETED
}