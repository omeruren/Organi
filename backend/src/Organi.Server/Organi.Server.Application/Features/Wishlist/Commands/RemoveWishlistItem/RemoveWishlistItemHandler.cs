using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Domain.Exceptions;

namespace Organi.Server.Application.Features.Wishlist.Commands.RemoveWishlistItem;

public sealed class RemoveWishlistItemHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser,
    ILogger<RemoveWishlistItemHandler> logger) : IRequestHandler<RemoveWishlistItemCommand>
{
    public async Task Handle(RemoveWishlistItemCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId!.Value;

        var item = await context.WishlistItems
            .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == request.ProductId, cancellationToken)
            ?? throw new NotFoundException("WishlistItem", request.ProductId);

        context.WishlistItems.Remove(item);
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Product {ProductId} removed from wishlist for user {UserId}", request.ProductId, userId);
    }
}
