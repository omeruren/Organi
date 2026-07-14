namespace Organi.Server.Application.Features.Products.DTOs;

public sealed record ProductSummaryResponse(
    Guid Id,
    string Name,
    string Slug,
    decimal Price,
    decimal? SalePrice,
    string Unit,
    bool IsOrganic,
    string Status,
    Guid CategoryId,
    string CategoryName,
    Guid VendorId,
    string VendorName,
    string? PrimaryImageUrl,
    decimal AverageRating);
