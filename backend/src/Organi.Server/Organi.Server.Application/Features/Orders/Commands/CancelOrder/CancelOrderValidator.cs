using FluentValidation;

namespace Organi.Server.Application.Features.Orders.Commands.CancelOrder;

public sealed class CancelOrderValidator : AbstractValidator<CancelOrderCommand>
{
    public CancelOrderValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Order ID is required.");

        RuleFor(x => x.Reason)
            .MaximumLength(500).WithMessage("Cancellation reason must not exceed 500 characters.");
    }
}
