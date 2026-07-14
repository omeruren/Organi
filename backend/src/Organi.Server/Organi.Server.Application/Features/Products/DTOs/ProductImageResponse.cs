namespace Organi.Server.Application.Features.Products.DTOs;

public sealed record ProductImageResponse(
    Guid Id,
    string ImageUrl,
    string? AltText,
    int DisplayOrder,
    bool IsPrimary);
