namespace Organi.Server.Application.Features.Products.DTOs;

public sealed record ProductImageRequest(
    string ImageUrl,
    string? AltText,
    int DisplayOrder,
    bool IsPrimary);
