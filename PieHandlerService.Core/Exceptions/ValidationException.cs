using FluentValidation.Results;
using Newtonsoft.Json;
using PieHandlerService.Core.Models;

namespace PieHandlerService.Core.Exceptions;

public class ValidationException : BaseException
{
    public ValidationException(ProblemDetails problemDetails) : this(problemDetails, new Exception()) { }

    public ValidationException(ProblemDetails problemDetails, Exception innerException)
        : this(problemDetails, new List<ValidationFailure>(), innerException) { }

    public ValidationException(ProblemDetails problemDetails, IEnumerable<ValidationFailure> validationFailures)
        : this(problemDetails, validationFailures, new Exception()) { }

    public ValidationException(
        ProblemDetails problemDetails,
        IEnumerable<ValidationFailure> validationFailures,
        Exception innerException) : base(problemDetails, innerException)
    {
        ProblemDetails = problemDetails;

        var enumerable = validationFailures as ValidationFailure[] ?? validationFailures.ToArray();
        if (!enumerable.Any())
        {
            return;
        }

        var failures = new Dictionary<string, string>();
        var propertyName = "";
        foreach (var failure in enumerable)
        {
            if (!failure.PropertyName.Equals(propertyName))
            {
                failures.Add(failure.PropertyName, failure.ErrorMessage);
            }
            propertyName = failure.PropertyName;
        }

        if (!failures.Any())
        {
            return;
        }

        if (problemDetails.MoreInfo == null)
        {
            ProblemDetails.MoreInfo = new Dictionary<string, object>();
        }

        ProblemDetails.MoreInfo.Add(nameof(ValidationFailure), failures);
    }

    public new int Code => ProblemDetails.Code;
    public new ProblemDetails ProblemDetails { get; }
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
