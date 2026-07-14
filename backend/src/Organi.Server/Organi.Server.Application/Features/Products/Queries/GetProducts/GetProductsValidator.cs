using FluentValidation;
using Organi.Server.Domain.Enums;

namespace Organi.Server.Application.Features.Products.Queries.GetProducts;

public sealed class GetProductsValidator : AbstractValidator<GetProductsQuery>
{
    private static readonly string[] AllowedSortFields = ["name", "price", "rating", "createdat"];
    private static readonly string[] AllowedSortOrders = ["asc", "desc"];

    public GetProductsValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("Page must be at least 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 50).WithMessage("Page size must be between 1 and 50.");

        RuleFor(x => x.SortBy)
            .Must(sortBy => AllowedSortFields.Contains(sortBy!.ToLowerInvariant()))
            .When(x => x.SortBy is not null)
            .WithMessage("SortBy must be one of: name, price, rating, createdAt.");

        RuleFor(x => x.SortOrder)
            .Must(sortOrder => AllowedSortOrders.Contains(sortOrder!.ToLowerInvariant()))
            .When(x => x.SortOrder is not null)
            .WithMessage("SortOrder must be 'asc' or 'desc'.");

        RuleFor(x => x.Status)
            .IsEnumName(typeof(ProductStatus), caseSensitive: false)
            .When(x => x.Status is not null)
            .WithMessage("Status must be a valid product status.");

        RuleFor(x => x.MinPrice)
            .LessThanOrEqualTo(x => x.MaxPrice)
            .When(x => x.MinPrice.HasValue && x.MaxPrice.HasValue)
            .WithMessage("MinPrice must not exceed MaxPrice.");
    }
}
