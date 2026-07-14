using FluentValidation;

namespace Organi.Server.Application.Features.Vendors.Commands.RegisterVendor;

public sealed class RegisterVendorValidator : AbstractValidator<RegisterVendorCommand>
{
    public RegisterVendorValidator()
    {
        RuleFor(x => x.StoreName)
            .NotEmpty().WithMessage("Store name is required.")
            .MaximumLength(200).WithMessage("Store name must not exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters.");

        RuleFor(x => x.LogoUrl)
            .MaximumLength(500).WithMessage("Logo URL must not exceed 500 characters.");

        RuleFor(x => x.BannerUrl)
            .MaximumLength(500).WithMessage("Banner URL must not exceed 500 characters.");

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20).WithMessage("Phone number must not exceed 20 characters.");

        RuleFor(x => x.Address)
            .MaximumLength(500).WithMessage("Address must not exceed 500 characters.");

        RuleFor(x => x.City)
            .MaximumLength(100).WithMessage("City must not exceed 100 characters.");
    }
}
