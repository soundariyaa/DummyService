using FluentValidation;
using PieHandlerService.Api.Contracts;

namespace PieHandlerService.Api.Validators;

public sealed class PieOrderSpecificationValidator : AbstractValidator<PieMessageSpecification>
{
    public PieOrderSpecificationValidator() {
        RuleFor(x => x.MixNumber).NotEmpty().NotNull();
        RuleFor(x => x.OeIdentifier).NotEmpty().NotNull();
        RuleFor(x => x.RequestType).NotEmpty().NotNull();
    } 

}