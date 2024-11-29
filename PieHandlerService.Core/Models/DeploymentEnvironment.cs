using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace PieHandlerService.Core.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DeploymentEnvironment
{
    [EnumMember(Value = "test")]
    Test,

    [EnumMember(Value = "qa")]
    Qa,

    [EnumMember(Value = "prod")]
    Prod
}