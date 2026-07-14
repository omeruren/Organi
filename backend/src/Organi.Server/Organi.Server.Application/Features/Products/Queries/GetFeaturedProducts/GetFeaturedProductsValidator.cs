using FluentValidation;

namespace Organi.Server.Application.Features.Products.Queries.GetFeaturedProducts;

public sealed class GetFeaturedProductsValidator : AbstractValidator<GetFeaturedProductsQuery>
{
    public GetFeaturedProductsValidator()
    {
        RuleFor(x => x.Take)
            .InclusiveBetween(1, 50).WithMessage("Take must be between 1 and 50.");
    }
}
