using FluentValidation;

namespace Organi.Server.Application.Features.Orders.Commands.CreateOrder;

public sealed class CreateOrderValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.ShippingFirstName)
            .NotEmpty().WithMessage("Shipping first name is required.")
            .MaximumLength(100).WithMessage("Shipping first name must not exceed 100 characters.");

        RuleFor(x => x.ShippingLastName)
            .NotEmpty().WithMessage("Shipping last name is required.")
            .MaximumLength(100).WithMessage("Shipping last name must not exceed 100 characters.");

        RuleFor(x => x.ShippingAddress)
            .NotEmpty().WithMessage("Shipping address is required.")
            .MaximumLength(500).WithMessage("Shipping address must not exceed 500 characters.");

        RuleFor(x => x.ShippingCity)
            .NotEmpty().WithMessage("Shipping city is required.")
            .MaximumLength(100).WithMessage("Shipping city must not exceed 100 characters.");

        RuleFor(x => x.ShippingPostalCode)
            .MaximumLength(20).WithMessage("Shipping postal code must not exceed 20 characters.");

        RuleFor(x => x.ShippingPhone)
            .NotEmpty().WithMessage("Shipping phone is required.")
            .MaximumLength(20).WithMessage("Shipping phone must not exceed 20 characters.");

        RuleFor(x => x.ShippingEmail)
            .NotEmpty().WithMessage("Shipping email is required.")
            .EmailAddress().WithMessage("Shipping email must be a valid email address.")
            .MaximumLength(256).WithMessage("Shipping email must not exceed 256 characters.");

        RuleFor(x => x.Notes)
            .MaximumLength(500).WithMessage("Notes must not exceed 500 characters.");

        RuleFor(x => x.CouponCode)
            .MaximumLength(50).WithMessage("Coupon code must not exceed 50 characters.");
    }
}
