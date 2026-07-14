using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Organi.Server.Application.Common.Interfaces;

namespace Organi.Server.Application.Features.Cart.Commands.ClearCart;

public sealed class ClearCartHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser,
    ILogger<ClearCartHandler> logger) : IRequestHandler<ClearCartCommand>
{
    public async Task Handle(ClearCartCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId!.Value;

        var cart = await context.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

        if (cart is null || cart.CartItems.Count == 0)
            return;

        context.CartItems.RemoveRange(cart.CartItems);
        cart.CartItems.Clear();
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Cart cleared for user {UserId}", userId);
    }
}
