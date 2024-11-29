using FluentValidation;
using PieHandlerService.Api.Contracts;

namespace PieHandlerService.Api.Validators;

public sealed class PreFlashOrderValidator : AbstractValidator<PreFlashOrderRequest>
{
    public PreFlashOrderValidator()
    {
        RuleFor(x => x.MixNumber).NotEmpty().NotNull();
        RuleFor(x => x.OriginHash).NotEmpty().NotNull();
        RuleFor(x => x.PreFlashContext).NotNull();
        When(x => x.PreFlashContext != null, () =>
        {
            RuleFor(x => x.PreFlashContext != null && x.PreFlashContext.Ecus != null);
        });
    }
}