using Microsoft.AspNetCore.Mvc;

namespace PieHandlerService.Api.Extensions;

internal static class ExceptionExtensions
{
    public static ProblemDetails ToProblemDetails400BadRequest(this Exception ex) =>
        new()
        {
            Title = "Bad Request",
            Status = StatusCodes.Status400BadRequest,
            Detail = ex.Message
        };

    public static ProblemDetails ToProblemDetails400BadRequest(this Exception ex, string title) =>
        new()
        {
            Title = title,
            Status = StatusCodes.Status400BadRequest,
            Detail = ex.Message
        };

    public static ProblemDetails ToProblemDetails408RequestTimeout(this Exception ex) =>
        new()
        {
            Title = "Request Timeout",
            Status = StatusCodes.Status408RequestTimeout,
            Detail = ex.Message
        };

    public static ProblemDetails ToProblemDetails408RequestTimeout(this Exception ex, string title) =>
        new()
        {
            Title = title,
            Status = StatusCodes.Status408RequestTimeout,
            Detail = ex.Message
        };

    public static ProblemDetails ToProblemDetails500InternalServerError(this Exception ex) =>
        new()
        {
            Title = "Request Timeout",
            Status = StatusCodes.Status500InternalServerError,
            Detail = ex.Message
        };

    public static ProblemDetails ToProblemDetails500InternalServerError(this Exception ex, string title) =>
        new()
        {
            Title = title,
            Status = StatusCodes.Status500InternalServerError,
            Detail = ex.Message
        };

    public static ProblemDetails ToProblemDetails(this Exception ex) =>
        new()
        {
            Title = "Internal Server Error",
            Status = StatusCodes.Status500InternalServerError,
            Detail = ex.Message
        };

    public static ProblemDetails ToProblemDetails(this Exception ex, string title) =>
        new()
        {
            Title = title,
            Status = StatusCodes.Status500InternalServerError,
            Detail = ex.Message
        };

    public static ProblemDetails ToProblemDetails(this Exception ex, int statusCode) =>
        new()
        {
            Title = ex.Message,
            Status = statusCode
        };
        

    public static ProblemDetails ToProblemDetails(this Exception ex, int statusCode, string title) =>
        new()
        {
            Title = title,
            Status = statusCode,
            Detail = ex.Message
        };

    public static ProblemDetails ToProblemDetails(this Exception ex, int statusCode, string title, string instance) =>
        new()
        {
            Title = title,
            Status = statusCode,
            Detail = ex.Message,
            Instance = instance
        };
    public static ProblemDetails ToProblemDetails(this Exception ex, int statusCode, string title, IDictionary<string, object?> extensions) =>
        new()
        {
            Title = title,
            Status = statusCode,
            Detail = ex.Message,
            Extensions = extensions
        };

    public static ProblemDetails ToProblemDetails(
        this Exception ex, int statusCode, string title, string instance, string type) =>
        new()
        {
            Title = title,
            Status = statusCode,
            Detail = ex.Message,
            Instance = instance,
            Type = type
        };

    public static ProblemDetails ToProblemDetails(
        this Exception ex, int statusCode, string title, string instance, string type, IDictionary<string, object?> extensions) =>
        new()
        {
            Title = title,
            Status = statusCode,
            Detail = ex.Message,
            Instance = instance,
            Type = type,
            Extensions = extensions
        };

    public static ProblemDetails CreateProblemDetailsErrorParsingFileContent(this Exception ex) =>
        new()
        {
            Title = "Error occurred while parsing the file content",
            Detail = ex.Message,
            Status = StatusCodes.Status400BadRequest
        };

    public static ProblemDetails CreateProblemDetailsErrorParsingFileContent(this Exception ex, string title) =>
        new()
        {
            Title = title,
            Detail = ex.Message,
            Status = StatusCodes.Status400BadRequest
        };

    public static ProblemDetails CreateProblemDetailsErrorParsingFileContent(this Exception _, string title, string detail) =>
        new()
        {
            Title = title,
            Detail = detail,
            Status = StatusCodes.Status400BadRequest
        };

    public static ProblemDetails CreateProblemDetailsErrorParsingContent(this Exception _, string title, string detail) =>
        new()
        {
            Title = title,
            Detail = detail,
            Status = StatusCodes.Status400BadRequest
        };
    public static ProblemDetails CreateProblemDetailsUnHandledException(this Exception ex) =>
        new()
        {
            Title = "An error occurred while processing your request.",
            Status = StatusCodes.Status500InternalServerError,
            Detail = ex.Message
        };

    public static ProblemDetails CreateProblemDetailsUnHandledException(this Exception ex, string requestPath) =>
        new()
        {
            Title = "An error occurred while processing your request.",
            Status = StatusCodes.Status500InternalServerError,
            Detail = ex.Message,
            Instance = requestPath
        };
}
