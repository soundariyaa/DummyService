using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace PieHandlerService.Core.Models;

public sealed class ProblemDetails
{
    public static readonly Dictionary<int, string> HttpStatusCodeTypeProvider = new Dictionary<int, string>
    {
        {StatusCodes.Status200OK, "https://tools.ietf.org/html/rfc7231#section-6.3.1"},
        {StatusCodes.Status202Accepted, "https://tools.ietf.org/html/rfc7231#section-6.3.3"},
        {StatusCodes.Status400BadRequest, "https://tools.ietf.org/html/rfc7231#section-6.5.1"},
        {StatusCodes.Status401Unauthorized, "https://tools.ietf.org/html/rfc7235#section-3.1"},
        {StatusCodes.Status403Forbidden, "https://tools.ietf.org/html/rfc7231#section-6.5.3"},
        {StatusCodes.Status404NotFound, "https://tools.ietf.org/html/rfc7231#section-6.5.4"},
        {StatusCodes.Status409Conflict, "https://tools.ietf.org/html/rfc7231#section-6.5.8"},
        {StatusCodes.Status422UnprocessableEntity, "https://tools.ietf.org/html/rfc4918#section-11.2"},
        {StatusCodes.Status429TooManyRequests, "https://tools.ietf.org/html/rfc6585#section-4"},
        {StatusCodes.Status500InternalServerError, "https://tools.ietf.org/html/rfc7231#section-6.6.1"},
        {StatusCodes.Status501NotImplemented, "https://tools.ietf.org/html/rfc7231#section-6.6.2"},
        {StatusCodes.Status502BadGateway, "https://tools.ietf.org/html/rfc7231#section-6.6.3"},
        {StatusCodes.Status503ServiceUnavailable, "https://tools.ietf.org/html/rfc7231#section-6.6.4"},
        {StatusCodes.Status504GatewayTimeout, "https://tools.ietf.org/html/rfc7231#section-6.6.5"}
    };

    public static class Codes
    {
        public static int NoErrors => 0;
        public static int Error10GeneralExternalSystemError => 10;
        public static int Error20BrokenCircuit => 20;
        public static int Error30ServiceUnavailable => 30;
        public static int Error40Unauthorizted => 40;
        public static int Error50Forbidden => 50;
        public static int Error104IncorrectConfiguredUser => 104;
        public static int Error200ProviderPie => 200;
        public static int Error900Validation => 900;
        public static int Error800DataStore => 800;
        public static int Error850DuplicateData => 850;
        public static int Error860VbfCheckError => 860;
        public static int Error1000UnhandledException => 1000;
        public static int Error1100RoutingErrorException => 1100;
        public static int Error1200BadRequest => 1200;
        public static int Error1300CommunicationFailure => 1300;
        public static int Error1400DataNotFound => 1400;
        public static int Error1800RequestCancellation => 1800;
        public static int Error400DataStore => 1900;

    }

    public static string GetStatusTypeOrEmptyString(int httpStatusCode)
    {
        return HttpStatusCodeTypeProvider.GetValueOrDefault(httpStatusCode) ?? string.Empty;
    }

    public int Status { get; set; } = StatusCodes.Status200OK;
    public int Code { get; set; } = Codes.NoErrors;
    public string? Title { get; set; }
    public string? Detail { get; set; }
    public string? Type { get; set; }
    public string? Instance { get; set; }
    public string? DocumentationUri { get; set; }
    public Dictionary<string, object> MoreInfo { get; set; } = new Dictionary<string, object>();
    [JsonIgnore]
    public int Provider { get; set; }

    public ProblemDetails() { }
    public ProblemDetails(int code,string detail, int status, string title) {
        Code = code;
        Detail = detail;
        Status = status;
        Title = title;
    }
    public ProblemDetails(ProblemDetails obj)
    {
        Code = obj.Code;
        Detail = obj.Detail;
        Instance = obj.Instance;
        Status = obj.Status;
        Title = obj.Title;
        Type = obj.Type;
        DocumentationUri = obj.DocumentationUri;
        MoreInfo = obj.MoreInfo;
    }

}