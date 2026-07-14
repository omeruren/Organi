using FluentValidation;
using Organi.Server.Domain.Enums;

namespace Organi.Server.Application.Features.Coupons.Commands.CreateCoupon;

public sealed class CreateCouponValidator : AbstractValidator<CreateCouponCommand>
{
    public CreateCouponValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Coupon code is required.")
            .MaximumLength(50).WithMessage("Coupon code must not exceed 50 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(256).WithMessage("Description must not exceed 256 characters.");

        RuleFor(x => x.DiscountType)
            .NotEmpty().WithMessage("Discount type is required.")
            .IsEnumName(typeof(DiscountType), caseSensitive: false)
            .WithMessage("Discount type must be 'Percentage' or 'FixedAmount'.");

        RuleFor(x => x.DiscountValue)
            .GreaterThan(0).WithMessage("Discount value must be greater than zero.");

        RuleFor(x => x)
            .Must(x => !string.Equals(x.DiscountType, nameof(DiscountType.Percentage), StringComparison.OrdinalIgnoreCase)
                       || x.DiscountValue <= 100)
            .WithMessage("Percentage discount cannot exceed 100.")
            .WithName("DiscountValue");

        RuleFor(x => x.MinimumOrderAmount)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MinimumOrderAmount.HasValue)
            .WithMessage("Minimum order amount must be zero or greater.");

        RuleFor(x => x.MaxUsageCount)
            .GreaterThan(0)
            .When(x => x.MaxUsageCount.HasValue)
            .WithMessage("Maximum usage count must be greater than zero.");

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate)
            .WithMessage("End date must be after the start date.");
    }
}
