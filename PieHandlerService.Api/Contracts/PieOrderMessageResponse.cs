using Newtonsoft.Json;
using PieHandlerService.Core.Models;

namespace PieHandlerService.Api.Contracts;

public class PieOrderMessageResponse : ResponseBase
{
    public PieOrderMessageResponse()
    {
        Status = StatusCodes.Status200OK;
    }

    public PieOrderMessageResponse(ProblemDetails problemDetails) : base(problemDetails) { }

    public IEnumerable<PieResponseMessage> PieResponseMessages { get; set; } = new List<PieResponseMessage>();

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this, new JsonSerializerSettings
        {
            DefaultValueHandling = DefaultValueHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore
        });
    }
}