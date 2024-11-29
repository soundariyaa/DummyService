using Newtonsoft.Json;
using PieHandlerService.Core.Models;

namespace PieHandlerService.Api.Contracts;

public sealed class PreFlashOrderResponse : ResponseBase
{
    public PreFlashOrderResponse() {
        Status = StatusCodes.Status200OK;
    }

    public string? OeIdentifier { get; set; }
    public string? MixNumber { get; set; }
    public Order? Order { get; set; }
    public string? KeyManifest { get; set; }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this, new JsonSerializerSettings
        {
            DefaultValueHandling = DefaultValueHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore
        });
    }
}