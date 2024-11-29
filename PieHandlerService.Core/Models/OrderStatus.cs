using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace PieHandlerService.Core.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum OrderStatus
{
    [EnumMember(Value = "Available")]
    Available,

    [EnumMember(Value = "Complete")]
    Complete,

    [EnumMember(Value = "Pending")]
    Pending,

}