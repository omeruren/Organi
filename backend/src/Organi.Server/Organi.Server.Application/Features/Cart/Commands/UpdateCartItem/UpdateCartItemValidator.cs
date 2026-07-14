using FluentValidation;

namespace Organi.Server.Application.Features.Cart.Commands.UpdateCartItem;

public sealed class UpdateCartItemValidator : AbstractValidator<UpdateCartItemCommand>
{
    public UpdateCartItemValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Cart item ID is required.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero.");
    }
}
