using FluentValidation;

namespace Organi.Server.Application.Features.Auth.Commands.ConfirmEmail;

public sealed class ConfirmEmailValidator : AbstractValidator<ConfirmEmailCommand>
{
    public ConfirmEmailValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Confirmation token is required.")
            .MaximumLength(256).WithMessage("Confirmation token is invalid.");
    }
}
