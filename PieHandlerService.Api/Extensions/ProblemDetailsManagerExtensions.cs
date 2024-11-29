using PieHandlerService.Core.Interfaces;
using PieHandlerService.Core.Models;
using PieHandlerService.Core.Extensions;

namespace PieHandlerService.Api.Extensions;

public static class ProblemDetailsManagerExtensions
{
    public static ProblemDetails GenerateRequestTerminatedProblemDetails(
        this IProblemDetailsManager problemDetailsManager,
        string methodName,
        Exception ex,
        ILogger logger)
    {
        var problemDetails = problemDetailsManager.KnownProblemDetails[ProblemDetails.Codes.Error1800RequestCancellation];
        logger.LogError($"{{MethodName}} RequestTerminated: {{@{nameof(ProblemDetails)}}} - {{{nameof(Exception)}}}",
            methodName, problemDetails.MakeMeDestructible(), ex.ToString());

        return problemDetails;
    }

    public static ProblemDetails GenerateUnhandledExceptionProblemDetails(
        this IProblemDetailsManager problemDetailsManager,
        string methodName,
        Exception ex,
        ILogger logger)
    {
        var problemDetails = problemDetailsManager.KnownProblemDetails[ProblemDetails.Codes.Error1000UnhandledException];
        logger.LogError($"{{MethodName}} UnhandledException: {{@{nameof(ProblemDetails)}}} - {{{nameof(Exception)}}}",
            methodName, problemDetails.MakeMeDestructible(), ex.ToString());

        return problemDetails;
    }
}
