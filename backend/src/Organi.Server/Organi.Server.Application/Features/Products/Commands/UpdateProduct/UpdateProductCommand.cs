using MediatR;
using Organi.Server.Application.Features.Products.DTOs;

namespace Organi.Server.Application.Features.Products.Commands.UpdateProduct;

public sealed record UpdateProductCommand(
    string Name,
    string? Description,
    string? ShortDescription,
    decimal Price,
    decimal? SalePrice,
    string SKU,
    int StockQuantity,
    string Unit,
    decimal? Weight,
    bool IsOrganic,
    bool IsFeatured,
    string Status,
    Guid CategoryId,
    IReadOnlyList<ProductImageRequest>? Images) : IRequest<ProductResponse>
{
    public Guid Id { get; init; }
}
