using System.Text.Json.Serialization;

namespace PieHandlerService.Core.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ConnectionType
{
    P2P,
    DiCE,
    J2534,
    Wifi,
    LAN,
    USB,
    Invehicle,
    Other
}