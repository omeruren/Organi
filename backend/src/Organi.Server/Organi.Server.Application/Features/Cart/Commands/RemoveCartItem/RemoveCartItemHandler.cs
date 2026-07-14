using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Domain.Exceptions;

namespace Organi.Server.Application.Features.Cart.Commands.RemoveCartItem;

public sealed class RemoveCartItemHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser,
    ILogger<RemoveCartItemHandler> logger) : IRequestHandler<RemoveCartItemCommand>
{
    public async Task Handle(RemoveCartItemCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId!.Value;

        var cart = await context.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken)
            ?? throw new NotFoundException("CartItem", request.Id);

        var item = cart.CartItems.FirstOrDefault(ci => ci.Id == request.Id)
            ?? throw new NotFoundException("CartItem", request.Id);

        context.CartItems.Remove(item);
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Cart item {CartItemId} removed for user {UserId}", item.Id, userId);
    }
}
