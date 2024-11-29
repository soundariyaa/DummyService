namespace PieHandlerService.Core.Models;

public abstract class ResultBase
{
    protected ResultBase() { }
    protected ResultBase(ProblemDetails problemDetails)
    {
        ProblemDetails = problemDetails ?? throw new ArgumentNullException(nameof(problemDetails));
    }

    public ProblemDetails ProblemDetails { get; set; } = new ProblemDetails();

    public abstract override string ToString();
}