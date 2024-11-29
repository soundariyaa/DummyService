using FluentValidation;
using PieHandlerService.Api.Contracts;

namespace PieHandlerService.Api.Validators;

public sealed class BroadcastContextSpecificationValidator : AbstractValidator<BroadcastMessageSpecification>
{
    public BroadcastContextSpecificationValidator() {
        RuleFor(x => x.MixNumber).NotEmpty();
        RuleFor(x => x.OeIdentifier).NotEmpty();
        RuleFor(x => x.RequestType).NotEmpty();
    }
}