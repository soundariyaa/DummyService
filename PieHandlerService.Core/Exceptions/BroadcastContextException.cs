using Newtonsoft.Json;
using PieHandlerService.Core.Models;


namespace PieHandlerService.Core.Exceptions;

public class BroadcastContextException(ProblemDetails problemDetails, Exception innerException)
    : BaseException(problemDetails, innerException)
{
    public static string DefaultMessageError => "Error Handling BroadcastContext Message or File";

    public BroadcastContextException(ProblemDetails problemDetails) : this(problemDetails, new Exception()) { }

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