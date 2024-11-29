using FluentValidation;
using PieHandlerService.Api.Contracts;

namespace PieHandlerService.Api.Validators;

public sealed class VehicleCodesValidator : AbstractValidator<VehicleCodesRequest>
{
    public VehicleCodesValidator()
    {

        RuleFor(x => x.MixNumber).NotEmpty().NotNull();
        RuleFor(x => x.OriginHash).NotEmpty().NotNull();
        RuleFor(x => x.VehicleObjectContext).NotNull();
        When(x => x.VehicleObjectContext != null, () =>
        {
            RuleFor(x => x.VehicleObjectContext != null && x.VehicleObjectContext.Vehicle != null);
        });
    }
}