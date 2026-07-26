using FluentValidation;
using Organi.Server.Application.Common.Validation;

namespace Organi.Server.Application.Features.Auth.Commands.ResetPassword;

public sealed class ResetPasswordValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email must be a valid email address.")
            .MaximumLength(256).WithMessage("Email must not exceed 256 characters.");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Reset code is required.")
            .Matches("^[0-9]{6}$").WithMessage("Reset code must be 6 digits.");

        RuleFor(x => x.NewPassword).Password();
    }
}
