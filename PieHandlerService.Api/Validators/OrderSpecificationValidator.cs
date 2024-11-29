using FluentValidation;
using PieHandlerService.Api.Contracts;

namespace PieHandlerService.Api.Validators;

public sealed class OrderSpecificationValidator : AbstractValidator<OrderSpecificationRequest>
{

    public OrderSpecificationValidator() {
        RuleFor(x => x.MixNumber).NotEmpty().NotNull();
        RuleFor(x => x.OeIdentifier).NotEmpty().NotNull();
        RuleFor(x => x.OrderType).NotEmpty().NotNull();
    } 

}