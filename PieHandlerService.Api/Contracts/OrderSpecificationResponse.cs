using Newtonsoft.Json;
using PieHandlerService.Core.Models;

namespace PieHandlerService.Api.Contracts;

public class OrderSpecificationResponse : ResponseBase
{

    public OrderSpecificationResponse()
    {
        Status = StatusCodes.Status200OK;
    }

    public OrderSpecificationResponse(ProblemDetails problemDetails) : base(problemDetails) { }

    public string? MixNumber { get; set; }

    public string? OeIdentifier { get; set; }

    public Order? PreFlashOrder { get; set; }

    public Order? EndOfLineOrder { get; set; }

    public Order? VehicleCodesOrder { get; set; }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this, new JsonSerializerSettings
        {
            DefaultValueHandling = DefaultValueHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore
        });
    }
}