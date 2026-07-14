using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Cart.DTOs;
using Organi.Server.Application.Features.Cart.Mappings;
using Organi.Server.Domain.Entities;
using Organi.Server.Domain.Exceptions;
using CartEntity = Organi.Server.Domain.Entities.Cart;

namespace Organi.Server.Application.Features.Cart.Commands.AddCartItem;

public sealed class AddCartItemHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser,
    ILogger<AddCartItemHandler> logger) : IRequestHandler<AddCartItemCommand, CartResponse>
{
    public async Task<CartResponse> Handle(AddCartItemCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId!.Value;

        var product = await context.Products.FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken)
            ?? throw new NotFoundException("Product", request.ProductId);

        var cart = await context.Carts
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Product)
            .ThenInclude(p => p.ProductImages)
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

        if (cart is null)
        {
            cart = new CartEntity { UserId = userId };
            context.Carts.Add(cart);
        }

        var existingItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == request.ProductId);
        var totalQuantity = (existingItem?.Quantity ?? 0) + request.Quantity;

        if (totalQuantity > product.StockQuantity)
            throw new BusinessRuleException(
                $"Insufficient stock for product '{product.Name}'. Available: {product.StockQuantity}, Requested: {totalQuantity}.");

        if (existingItem is not null)
        {
            existingItem.Quantity = totalQuantity;
        }
        else
        {
            var newItem = new CartItem
            {
                ProductId = product.Id,
                Product = product,
                Quantity = request.Quantity,
                UnitPrice = product.SalePrice ?? product.Price,
                CartId = cart.Id
            };

            // Explicitly Add via the DbSet (not cart.CartItems.Add): `cart` may already be tracked
            // as Unchanged (existing cart), and this CartItem's Guid key is already set — EF would
            // otherwise assume it's an existing row. EF's relationship fixup adds it to
            // cart.CartItems on its own; adding it there manually too would double it up in-memory.
            context.CartItems.Add(newItem);
        }

        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Product {ProductId} added to cart for user {UserId}", product.Id, userId);

        return cart.ToResponse();
    }
}
