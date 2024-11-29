using PieHandlerService.Core.Models;
using System.Text;

namespace PieHandlerService.Core.Exceptions;

public class BaseException : Exception
{
    protected BaseException(string title, ProblemDetails problemDetails) : this(problemDetails, new Exception()) { }

    protected BaseException(ProblemDetails problemDetails, Exception innerException) : base(problemDetails.Title,
        innerException)
    {
        ProblemDetails = problemDetails;
    }

    public int Code => ProblemDetails.Code;

    public int StatusCode => ProblemDetails.Status;

    public ProblemDetails ProblemDetails { get; protected set; }

    public override string ToString()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append(ProblemDetails);
        stringBuilder.Append(" - ");
        stringBuilder.AppendLine();
        stringBuilder.Append(base.ToString());
        return stringBuilder.ToString();
    }

    public string ToShortString()
    {
        const int maximumAllowedCharacters = 500;
        const char blankSpace = ' ';
        var stringBuilder = new StringBuilder();
        stringBuilder.Append("Status: ").Append(ProblemDetails.Status);
        stringBuilder.Append(blankSpace);
        stringBuilder.Append("Code: ").Append(ProblemDetails.Code);
        if (string.IsNullOrEmpty(ProblemDetails.Title)) return stringBuilder.ToString();
        stringBuilder.Append(blankSpace);
        stringBuilder.Append("Title: ").Append(ProblemDetails.Title);
        var customizedMessage = stringBuilder.ToString();
        return customizedMessage.Length > maximumAllowedCharacters
            ? customizedMessage[..maximumAllowedCharacters]
            : customizedMessage;
    }
}