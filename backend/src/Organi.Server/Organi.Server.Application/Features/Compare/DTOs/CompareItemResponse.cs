namespace Organi.Server.Application.Features.Compare.DTOs;

public sealed record CompareItemResponse(
    Guid Id,
    Guid ProductId,
    string ProductName,
    string ProductSlug,
    decimal Price,
    decimal? SalePrice,
    string? ImageUrl,
    DateTime CreatedAt);
