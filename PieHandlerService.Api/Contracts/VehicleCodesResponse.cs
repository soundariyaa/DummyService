using Newtonsoft.Json;
using PieHandlerService.Core.Models;

namespace PieHandlerService.Api.Contracts;

public sealed class VehicleCodesResponse : ResponseBase
{
    public VehicleCodesResponse() {
        Status = StatusCodes.Status200OK;
    }

    public string OriginHash { get; set; } = string.Empty;
    public string OeIdentifier { get; set; } = string.Empty;
    public string MixNumber { get; set; } = string.Empty;
    public string PackageIdentity { get; set; } = string.Empty;
    public Order? Order { get; set; } = new Order();
    public string? StorageLocation { get; set; } = string.Empty;
    public string KeyManifest { get; set; } = string.Empty;

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this, new JsonSerializerSettings
        {
            DefaultValueHandling = DefaultValueHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore
        });
    }
}