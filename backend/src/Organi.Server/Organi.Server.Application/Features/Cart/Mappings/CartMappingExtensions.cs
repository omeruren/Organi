using Organi.Server.Application.Features.Cart.DTOs;
using CartEntity = Organi.Server.Domain.Entities.Cart;

namespace Organi.Server.Application.Features.Cart.Mappings;

public static class CartMappingExtensions
{
    public static CartResponse ToResponse(this CartEntity cart)
    {
        var items = cart.CartItems
            .Select(ci => new CartItemResponse(
                ci.Id,
                ci.ProductId,
                ci.Product.Name,
                ci.Product.Slug,
                ci.UnitPrice,
                ci.Quantity,
                ci.UnitPrice * ci.Quantity,
                ci.Product.ProductImages.Where(i => i.IsPrimary).Select(i => i.ImageUrl).FirstOrDefault()))
            .ToList();

        return new CartResponse(cart.Id, items, items.Sum(i => i.LineTotal));
    }
}
