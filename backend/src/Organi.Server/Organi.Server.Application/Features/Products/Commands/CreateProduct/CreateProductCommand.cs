using MediatR;
using Organi.Server.Application.Features.Products.DTOs;

namespace Organi.Server.Application.Features.Products.Commands.CreateProduct;

public sealed record CreateProductCommand(
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
    Guid CategoryId,
    IReadOnlyList<ProductImageRequest>? Images) : IRequest<ProductResponse>;
