using FluentValidation;
using Organi.Server.Domain.Enums;

namespace Organi.Server.Application.Features.Vendors.Queries.GetVendorDashboardOrders;

public sealed class GetVendorDashboardOrdersValidator : AbstractValidator<GetVendorDashboardOrdersQuery>
{
    public GetVendorDashboardOrdersValidator()
    {
        RuleFor(x => x.Status)
            .IsEnumName(typeof(OrderStatus), caseSensitive: false)
            .When(x => x.Status is not null)
            .WithMessage("Status must be a valid order status.");

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("Page must be at least 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 50).WithMessage("Page size must be between 1 and 50.");
    }
}
