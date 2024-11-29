using Microsoft.Extensions.Logging;
using PieHandlerService.Core.Interfaces;
using PieHandlerService.Core.Models;
using PieHandlerService.Infrastructure.Services.Pie.Contracts;

namespace PieHandlerService.Infrastructure.Services.Pie;

internal class ProblemDetailsHandlerBase<T>
{
    public string Target => "Pie";

    protected readonly IProblemDetailsManager ProblemDetailsManager;
    protected readonly ILogger<T> Logger;

    public IReadOnlyDictionary<int, int> TargetReturnCodeCorrelation { get; }

    protected ProblemDetailsHandlerBase(
        IProblemDetailsManager problemDetailsManager,
        ILogger<T> logger)
    {
        ProblemDetailsManager = problemDetailsManager ?? throw new ArgumentNullException(nameof(problemDetailsManager));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));

        TargetReturnCodeCorrelation = new Dictionary<int, int>
        {
            { 100, ProblemDetails.Codes.Error10GeneralExternalSystemError },
            { 400, ProblemDetails.Codes.Error1200BadRequest },
            { 404, ProblemDetails.Codes.Error1400DataNotFound }
        };
    }

    protected virtual ProblemDetails GetProblemDetails(ResponseBase responseBase, StatusCode statusCode)
    {
        Logger.LogError("{MethodName} with {ResponseType} Request Failed: " +
            "ReturnCode {Code}, Message {ErrorDetail}",
            nameof(GetProblemDetails), responseBase.GetType().Name,
            responseBase.Code, responseBase?.ErrorDetail() ?? string.Empty);

        ProblemDetails problemDetails;
        if (null!= responseBase && !responseBase.IsEmpty() &&
            TargetReturnCodeCorrelation.ContainsKey(responseBase.Code) &&
            ProblemDetailsManager.KnownProblemDetails.ContainsKey(TargetReturnCodeCorrelation[responseBase.Code]))
        {
            problemDetails = ProblemDetailsManager.KnownProblemDetails[TargetReturnCodeCorrelation[responseBase.Code]];
            problemDetails.MoreInfo.Add(responseBase.GetType().Name, new
            {
                Target,
                Status = statusCode?.Value,
                Code = responseBase.Code,
                responseBase?.Title,
                Detail = responseBase?.ErrorDetail()
            });
        }
        else
        {
            problemDetails = ProblemDetailsManager.KnownProblemDetails[ProblemDetails.Codes.Error10GeneralExternalSystemError];
        }
        problemDetails.Provider = ProblemDetails.Codes.Error200ProviderPie;
        return problemDetails;
    }
}
