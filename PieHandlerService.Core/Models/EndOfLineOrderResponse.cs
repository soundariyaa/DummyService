﻿

using Newtonsoft.Json;

namespace PieHandlerService.Core.Models;

public class EndOfLineOrderResponse : ResultBase
{

    public EndOfLineOrderResponse() { 
        
    }
    public string OriginHash { get; set; } = string.Empty;
    public string OeIdentifier { get; set; } = string.Empty;
    public string MixNumber { get; set; } = string.Empty;
    public Order? Order { get; set; } = new Order();
    public string? PieKeyManifest { get; set; }
    public string? StorageLocation { get; set; } = string.Empty;

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