using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PieHandlerService.Core.Models;

[JsonConverter(typeof(StringEnumConverter))]
public enum EcuStatus
{
    E,
    D,
    I,
    P
}