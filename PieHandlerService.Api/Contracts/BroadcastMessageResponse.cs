using Newtonsoft.Json;
using PieHandlerService.Core.Models;

namespace PieHandlerService.Api.Contracts;

public sealed class BroadcastMessageResponse : ResponseBase
{

    public BroadcastMessageResponse()
    {
        Status = StatusCodes.Status200OK;
    }

    public BroadcastMessageResponse(ProblemDetails problemDetails) : base(problemDetails) { }

    public IEnumerable<BroadcastContextMessage>? BroadcastContextMessages { get; set; }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this, new JsonSerializerSettings
        {
            DefaultValueHandling = DefaultValueHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore
        });
    }
}