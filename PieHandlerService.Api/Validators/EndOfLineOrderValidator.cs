using FluentValidation;
using PieHandlerService.Api.Contracts;

namespace PieHandlerService.Api.Validators;

public sealed class EndOfLineOrderValidator : AbstractValidator<EOLOrderRequest>
{
    public EndOfLineOrderValidator()
    {
        RuleFor(x => x.MixNumber).NotEmpty().NotNull();
        RuleFor(x => x.OriginHash).NotEmpty().NotNull();
        RuleFor(x => x.EndOfLineContext).NotNull();
        When(x => x.EndOfLineContext != null, () =>
        {
            RuleFor(x => x.EndOfLineContext != null && x.EndOfLineContext.Ecus != null);
        });
    }
}