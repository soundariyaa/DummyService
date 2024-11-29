

using Newtonsoft.Json;

namespace PieHandlerService.Core.Models;

public class VehicleCodesResponse : ResultBase
{
    public VehicleCodesResponse() { 
        
    }

    public VehicleCodesResponse(Order Order, string KeyManifest, string packageIdentity)
    {
        this.Order = Order;
        this.KeyManifest = KeyManifest;
        this.PackageIdentity = packageIdentity;

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
        return JsonConvert.SerializeObject(
            this,
            new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            });
    }
}