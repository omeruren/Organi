using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Cart.DTOs;
using Organi.Server.Application.Features.Cart.Mappings;
using Organi.Server.Domain.Exceptions;

namespace Organi.Server.Application.Features.Cart.Commands.UpdateCartItem;

public sealed class UpdateCartItemHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser,
    ILogger<UpdateCartItemHandler> logger) : IRequestHandler<UpdateCartItemCommand, CartResponse>
{
    public async Task<CartResponse> Handle(UpdateCartItemCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId!.Value;

        var cart = await context.Carts
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Product)
            .ThenInclude(p => p.ProductImages)
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken)
            ?? throw new NotFoundException("CartItem", request.Id);

        var item = cart.CartItems.FirstOrDefault(ci => ci.Id == request.Id)
            ?? throw new NotFoundException("CartItem", request.Id);

        if (request.Quantity > item.Product.StockQuantity)
            throw new BusinessRuleException(
                $"Insufficient stock for product '{item.Product.Name}'. Available: {item.Product.StockQuantity}, Requested: {request.Quantity}.");

        item.Quantity = request.Quantity;
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Cart item {CartItemId} updated for user {UserId}", item.Id, userId);

        return cart.ToResponse();
    }
}
