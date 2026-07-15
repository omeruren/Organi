using Organi.Server.Application.Features.Wishlist.DTOs;
using Organi.Server.Domain.Entities;

namespace Organi.Server.Application.Features.Wishlist.Mappings;

public static class WishlistMappingExtensions
{
    public static WishlistItemResponse ToResponse(this WishlistItem item) => new(
        item.Id,
        item.ProductId,
        item.Product.Name,
        item.Product.Slug,
        item.Product.Price,
        item.Product.SalePrice,
        item.Product.ProductImages.Where(i => i.IsPrimary).Select(i => i.ImageUrl).FirstOrDefault(),
        item.CreatedAt);
}
