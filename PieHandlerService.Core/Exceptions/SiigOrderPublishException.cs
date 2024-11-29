using Newtonsoft.Json;
using PieHandlerService.Core.Models;


namespace PieHandlerService.Core.Exceptions;

public class SiigOrderPublishException(ProblemDetails problemDetails, Exception innerException)
    : BaseException(problemDetails, innerException)
{
    public SiigOrderPublishException(ProblemDetails problemDetails) : this(problemDetails, new Exception()) { }

    public new int Code => ProblemDetails.Code;
    public new ProblemDetails ProblemDetails { get; } = problemDetails;

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