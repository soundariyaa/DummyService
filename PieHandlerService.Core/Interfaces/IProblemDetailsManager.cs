using PieHandlerService.Core.Models;

namespace PieHandlerService.Core.Interfaces;

public interface IProblemDetailsManager
{
    IReadOnlyDictionary<int, ProblemDetails> KnownProblemDetails { get; }
}
