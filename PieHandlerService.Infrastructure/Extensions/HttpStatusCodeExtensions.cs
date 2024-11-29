using System.Net;
using PieHandlerService.Core.Interfaces;
using PieHandlerService.Core.Models;

namespace PieHandlerService.Infrastructure.Extensions;

internal static class HttpStatusCodeExtensions
{
    public static bool TryMapToCommonProblemDetails(
        this HttpStatusCode httpStatusCode,
        IProblemDetailsManager problemDetailsManager,
        out ProblemDetails problemDetails)
    {
        problemDetails = httpStatusCode switch
        {
            HttpStatusCode.BadRequest => problemDetailsManager.KnownProblemDetails[ProblemDetails.Codes.Error1200BadRequest],
            HttpStatusCode.Unauthorized => problemDetailsManager.KnownProblemDetails[ProblemDetails.Codes.Error40Unauthorizted],
            HttpStatusCode.Forbidden => problemDetailsManager.KnownProblemDetails[ProblemDetails.Codes.Error50Forbidden],
            HttpStatusCode.NotFound => problemDetailsManager.KnownProblemDetails[ProblemDetails.Codes.Error1400DataNotFound],
            HttpStatusCode.InternalServerError or HttpStatusCode.BadGateway => 
                problemDetailsManager.KnownProblemDetails[ProblemDetails.Codes.Error1300CommunicationFailure],
            HttpStatusCode.ServiceUnavailable => problemDetailsManager.KnownProblemDetails[ProblemDetails.Codes.Error30ServiceUnavailable],
            _ => throw new NotImplementedException()
        };
        return problemDetails != null;
    }
}
