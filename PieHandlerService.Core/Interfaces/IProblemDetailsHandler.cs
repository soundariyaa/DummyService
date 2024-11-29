using PieHandlerService.Core.Models;

namespace PieHandlerService.Core.Interfaces;

public interface IProblemDetailsHandler<in TIn>
{
    ProblemDetails Handle(TIn input);
}
