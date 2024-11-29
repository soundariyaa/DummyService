using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace PieHandlerService.Core.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum EncryptionType
{
    [EnumMember(Value = "NULL")]
    Null,
    [EnumMember(Value = "STATIC")]
    Static,
    [EnumMember(Value = "VIN_UNIQUE")]
    VinUnique
}