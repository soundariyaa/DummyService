using Microsoft.AspNetCore.Http;
using PieHandlerService.Core.Interfaces;
using PieHandlerService.Core.Models;


namespace PieHandlerService.Core;

public sealed class ProblemDetailsManager(string documentationUri) : IProblemDetailsManager
{
    public ProblemDetailsManager() : this(string.Empty) { }

    public IReadOnlyDictionary<int, ProblemDetails> KnownProblemDetails => new Dictionary<int, ProblemDetails>
    {
        {
            ProblemDetails.Codes.NoErrors,
            new ProblemDetails
            {
                Code = ProblemDetails.Codes.NoErrors,
                Title = "No Error",
                Status = StatusCodes.Status200OK,
                Type = ProblemDetails.GetStatusTypeOrEmptyString(StatusCodes.Status200OK),
                DocumentationUri = documentationUri
            }
        },
        {
            ProblemDetails.Codes.Error10GeneralExternalSystemError,
            new ProblemDetails
            {
                Code = ProblemDetails.Codes.Error10GeneralExternalSystemError,
                Title = "External System Error",
                Status = StatusCodes.Status502BadGateway,
                Type = ProblemDetails.GetStatusTypeOrEmptyString(StatusCodes.Status502BadGateway),
                DocumentationUri = documentationUri
            }
        },
        {
            ProblemDetails.Codes.Error20BrokenCircuit,
            new ProblemDetails
            {
                Code = ProblemDetails.Codes.Error20BrokenCircuit,
                Title = "Circuit Breaker Open",
                Status = StatusCodes.Status503ServiceUnavailable,
                Type = ProblemDetails.GetStatusTypeOrEmptyString(StatusCodes.Status503ServiceUnavailable),
                DocumentationUri = documentationUri
            }
        },
        {
            ProblemDetails.Codes.Error30ServiceUnavailable,
            new ProblemDetails
            {
                Code = ProblemDetails.Codes.Error30ServiceUnavailable,
                Title = "Service Unavailable",
                Status = StatusCodes.Status503ServiceUnavailable,
                Type = ProblemDetails.GetStatusTypeOrEmptyString(StatusCodes.Status503ServiceUnavailable),
                DocumentationUri = documentationUri
            }
        },
        {
            ProblemDetails.Codes.Error40Unauthorizted,
            new ProblemDetails
            {
                Code = ProblemDetails.Codes.Error40Unauthorizted,
                Title = "Unauthorized",
                Status = StatusCodes.Status401Unauthorized,
                Type = ProblemDetails.GetStatusTypeOrEmptyString(StatusCodes.Status401Unauthorized),
                DocumentationUri = documentationUri
            }
        },
        {
            ProblemDetails.Codes.Error50Forbidden,
            new ProblemDetails
            {
                Code = ProblemDetails.Codes.Error50Forbidden,
                Title = "Forbidden",
                Status = StatusCodes.Status403Forbidden,
                Type = ProblemDetails.GetStatusTypeOrEmptyString(StatusCodes.Status403Forbidden),
                DocumentationUri = documentationUri
            }
        },
        {
            ProblemDetails.Codes.Error200ProviderPie,
            new ProblemDetails
            {
                Code = ProblemDetails.Codes.Error200ProviderPie,
                Title = "PIE Error",
                Status = StatusCodes.Status502BadGateway,
                Type = ProblemDetails.GetStatusTypeOrEmptyString(StatusCodes.Status502BadGateway),
                DocumentationUri = documentationUri
            }
        },
        {
            ProblemDetails.Codes.Error800DataStore,
            new ProblemDetails
            {
                Code = ProblemDetails.Codes.Error800DataStore,
                Title = "Datastore Error",
                Status = StatusCodes.Status502BadGateway,
                Type = ProblemDetails.GetStatusTypeOrEmptyString(StatusCodes.Status502BadGateway),
                DocumentationUri = documentationUri
            }
        },
                {
            ProblemDetails.Codes.Error850DuplicateData,
            new ProblemDetails
            {
                Code = ProblemDetails.Codes.Error850DuplicateData,
                Title = "Duplicate Data",
                Status = StatusCodes.Status409Conflict,
                Type = ProblemDetails.GetStatusTypeOrEmptyString(StatusCodes.Status409Conflict),
                DocumentationUri = documentationUri
            }
        },
                                {
            ProblemDetails.Codes.Error860VbfCheckError,
            new ProblemDetails
            {
                Code = ProblemDetails.Codes.Error860VbfCheckError,
                Title = "Mismatch in NAS Vbf files and PIE Response",
                Status = StatusCodes.Status409Conflict,
                Type = ProblemDetails.GetStatusTypeOrEmptyString(StatusCodes.Status409Conflict),
                DocumentationUri = documentationUri
            }
        },
        {
            ProblemDetails.Codes.Error900Validation,
            new ProblemDetails
            {
                Code = ProblemDetails.Codes.Error900Validation,
                Title = "Validation Error",
                Status = StatusCodes.Status400BadRequest,
                Type = ProblemDetails.GetStatusTypeOrEmptyString(StatusCodes.Status400BadRequest),
                DocumentationUri = documentationUri
            }
        },
        {
            ProblemDetails.Codes.Error1000UnhandledException,
            new ProblemDetails
            {
                Code = ProblemDetails.Codes.Error1000UnhandledException,
                Title = "Unhandled Exception",
                Status = StatusCodes.Status500InternalServerError,
                Type = ProblemDetails.GetStatusTypeOrEmptyString(StatusCodes.Status500InternalServerError),
                DocumentationUri = documentationUri
            }
        },
        {
            ProblemDetails.Codes.Error1100RoutingErrorException,
            new ProblemDetails
            {
                Code = ProblemDetails.Codes.Error1100RoutingErrorException,
                Title = "Failed to get routing result",
                Status = StatusCodes.Status400BadRequest,
                Type = ProblemDetails.GetStatusTypeOrEmptyString(StatusCodes.Status400BadRequest),
                DocumentationUri = documentationUri
            }
        },
        {
            ProblemDetails.Codes.Error1200BadRequest,
            new ProblemDetails
            {
                Code = ProblemDetails.Codes.Error1200BadRequest,
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Type = ProblemDetails.GetStatusTypeOrEmptyString(StatusCodes.Status400BadRequest),
                DocumentationUri = documentationUri
            }
        },
        {
            ProblemDetails.Codes.Error1300CommunicationFailure,
            new ProblemDetails
            {
                Code = ProblemDetails.Codes.Error1300CommunicationFailure,
                Title = "Communication Failure",
                Status = StatusCodes.Status502BadGateway,
                Type = ProblemDetails.GetStatusTypeOrEmptyString(StatusCodes.Status502BadGateway),
                DocumentationUri = documentationUri
            }
        },
        {
            ProblemDetails.Codes.Error1400DataNotFound,
            new ProblemDetails
            {
                Code = ProblemDetails.Codes.Error1400DataNotFound,
                Title = "Data Not Found",
                Status = StatusCodes.Status404NotFound,
                Type = ProblemDetails.GetStatusTypeOrEmptyString(StatusCodes.Status404NotFound),
                DocumentationUri = documentationUri
            }
        },
        {
            ProblemDetails.Codes.Error1800RequestCancellation,
            new ProblemDetails
            {
                Code = ProblemDetails.Codes.Error1800RequestCancellation,
                Title = "Request execution canceled. " +
                        "Either due to global timeout expiration before execution completion or caller terminating executing thread",
                Status = StatusCodes.Status503ServiceUnavailable,
                Type = ProblemDetails.GetStatusTypeOrEmptyString(StatusCodes.Status503ServiceUnavailable),
                DocumentationUri = documentationUri
            }
        }
    };
}
