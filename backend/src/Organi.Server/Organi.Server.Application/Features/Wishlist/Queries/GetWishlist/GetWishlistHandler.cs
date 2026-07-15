using MediatR;
using Microsoft.EntityFrameworkCore;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Wishlist.DTOs;
using Organi.Server.Application.Features.Wishlist.Mappings;

namespace Organi.Server.Application.Features.Wishlist.Queries.GetWishlist;

public sealed class GetWishlistHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser) : IRequestHandler<GetWishlistQuery, IReadOnlyList<WishlistItemResponse>>
{
    public async Task<IReadOnlyList<WishlistItemResponse>> Handle(GetWishlistQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId!.Value;

        // Queries must not modify state (ADR-002) — a user with an empty wishlist simply
        // gets an empty list; AddWishlistItemHandler is responsible for creating rows.
        var items = await context.WishlistItems
            .AsNoTracking()
            .Where(w => w.UserId == userId)
            .Include(w => w.Product)
            .ThenInclude(p => p.ProductImages)
            .OrderByDescending(w => w.CreatedAt)
            .ToListAsync(cancellationToken);

        return items.Select(i => i.ToResponse()).ToList();
    }
}
