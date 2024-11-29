using Microsoft.AspNetCore.Mvc;

namespace PieHandlerService.Api.Extensions;

internal static class StringExtensions
{
    public static ProblemDetails ToProblemDetails(this string message, int statusCode) =>
        new () { Title = message, Status = statusCode };
    public static ProblemDetails ToProblemDetails(this string message, int statusCode, IDictionary<string, object?> extensions) =>
        new () { Title = message, Status = statusCode, Extensions = extensions };

    public static ProblemDetails ToProblemDetails(this string message, int statusCode, string detail) =>
        new () { Title = message, Status = statusCode, Detail = detail };

    public static ProblemDetails ToProblemDetails(this string message, int statusCode, string detail, string instance) =>
        new () { Title = message, Status = statusCode, Detail = detail, Instance = instance };

    public static ProblemDetails ToProblemDetails(this string message, int statusCode, string detail, string instance, string type) =>
        new () { Title = message, Status = statusCode, Detail = detail, Instance = instance, Type = type };

    public static ProblemDetails ToProblemDetails(
        this string message, int statusCode, string detail, string instance, string type, IDictionary<string, object?> extensions) =>
        new() { Title = message, Status = statusCode, Detail = detail, Instance = instance, Type = type, Extensions = extensions };

    public static ProblemDetails ToProblemDetailsArtifactNotFound(this string message) =>
        new () { Title = message, Status = StatusCodes.Status404NotFound };
}
