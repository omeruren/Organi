namespace Organi.Server.Application.Features.Wishlist.DTOs;

public sealed record WishlistItemResponse(
    Guid Id,
    Guid ProductId,
    string ProductName,
    string ProductSlug,
    decimal Price,
    decimal? SalePrice,
    string? ImageUrl,
    DateTime CreatedAt);
