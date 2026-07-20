using FluentValidation;
using Organi.Server.Domain.Enums;

namespace Organi.Server.Application.Features.Vendors.Queries.GetVendors;

public sealed class GetVendorsValidator : AbstractValidator<GetVendorsQuery>
{
    public GetVendorsValidator()
    {
        RuleFor(x => x.Status)
            .IsEnumName(typeof(VendorStatus), caseSensitive: false)
            .When(x => x.Status is not null)
            .WithMessage("Status must be a valid vendor status.");

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("Page must be at least 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 50).WithMessage("Page size must be between 1 and 50.");
    }
}
