using PieHandlerService.Core.Models;

namespace PieHandlerService.Api.Contracts;

public abstract class ResponseBase
{
    protected ResponseBase() { }

    protected ResponseBase(ProblemDetails problemDetails)
    {
        if (problemDetails == null)
        {
            throw new ArgumentNullException(nameof(problemDetails));
        }

        Status = problemDetails.Status;
        Code = problemDetails.Code;
        Title = problemDetails.Title;
        Detail = problemDetails.Detail;
        Type = problemDetails.Type;
        Instance = problemDetails.Instance;
        DocumentationUri = problemDetails.DocumentationUri;
        MoreInfo = problemDetails.MoreInfo?.Count == 0 ? null : problemDetails.MoreInfo;
    }

    public string? Type { get; set; }
    public int Status { get; set; } = StatusCodes.Status200OK;
    public int? Code { get; set; } = ProblemDetails.Codes.NoErrors;
    public string? Title { get; set; }
    public string? Detail { get; set; }
    public string? Instance { get; set; }
    public string? DocumentationUri { get; set; }
    public Dictionary<string, object>? MoreInfo { get; set; }

    public abstract override string ToString();
}
