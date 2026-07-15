using FluentValidation;

namespace Organi.Server.Application.Features.Newsletter.Commands.SubscribeNewsletter;

public sealed class SubscribeNewsletterValidator : AbstractValidator<SubscribeNewsletterCommand>
{
    public SubscribeNewsletterValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .MaximumLength(256).WithMessage("Email must not exceed 256 characters.")
            .EmailAddress().WithMessage("A valid email address is required.");
    }
}
