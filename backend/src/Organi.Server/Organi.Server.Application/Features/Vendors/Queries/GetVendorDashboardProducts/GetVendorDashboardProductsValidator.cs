using FluentValidation;

namespace Organi.Server.Application.Features.Vendors.Queries.GetVendorDashboardProducts;

public sealed class GetVendorDashboardProductsValidator : AbstractValidator<GetVendorDashboardProductsQuery>
{
    public GetVendorDashboardProductsValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("Page must be at least 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 50).WithMessage("Page size must be between 1 and 50.");
    }
}
