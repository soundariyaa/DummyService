using Newtonsoft.Json;
using PieHandlerService.Core.Models;

namespace PieHandlerService.Api.Contracts;

public sealed class AdminResponse : ResponseBase
{
    public AdminResponse()
    {
        Status = StatusCodes.Status200OK;
    }

    public AdminResponse(ProblemDetails problemDetails) : base(problemDetails) { }

    public bool IsComplete { get; set; }
    public bool IsStarted { get; set; }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this, new JsonSerializerSettings
        {
            DefaultValueHandling = DefaultValueHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore
        });
    }
}