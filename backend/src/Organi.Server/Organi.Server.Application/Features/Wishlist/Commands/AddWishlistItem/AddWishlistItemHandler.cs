using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Wishlist.DTOs;
using Organi.Server.Application.Features.Wishlist.Mappings;
using Organi.Server.Domain.Entities;
using Organi.Server.Domain.Exceptions;

namespace Organi.Server.Application.Features.Wishlist.Commands.AddWishlistItem;

public sealed class AddWishlistItemHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser,
    ILogger<AddWishlistItemHandler> logger) : IRequestHandler<AddWishlistItemCommand, WishlistItemResponse>
{
    public async Task<WishlistItemResponse> Handle(AddWishlistItemCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId!.Value;

        var product = await context.Products
            .Include(p => p.ProductImages)
            .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken)
            ?? throw new NotFoundException("Product", request.ProductId);

        var existingItem = await context.WishlistItems
            .Include(w => w.Product)
            .ThenInclude(p => p.ProductImages)
            .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == request.ProductId, cancellationToken);

        if (existingItem is not null)
            return existingItem.ToResponse();

        var item = new WishlistItem
        {
            UserId = userId,
            ProductId = product.Id,
            Product = product
        };

        context.WishlistItems.Add(item);
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Product {ProductId} added to wishlist for user {UserId}", product.Id, userId);

        return item.ToResponse();
    }
}
