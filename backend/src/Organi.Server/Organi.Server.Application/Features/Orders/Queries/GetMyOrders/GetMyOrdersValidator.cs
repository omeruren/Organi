using FluentValidation;

namespace Organi.Server.Application.Features.Orders.Queries.GetMyOrders;

public sealed class GetMyOrdersValidator : AbstractValidator<GetMyOrdersQuery>
{
    public GetMyOrdersValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("Page must be at least 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 50).WithMessage("Page size must be between 1 and 50.");
    }
}
