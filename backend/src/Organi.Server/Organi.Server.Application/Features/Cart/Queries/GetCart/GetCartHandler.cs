using MediatR;
using Microsoft.EntityFrameworkCore;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Cart.DTOs;
using Organi.Server.Application.Features.Cart.Mappings;

namespace Organi.Server.Application.Features.Cart.Queries.GetCart;

public sealed class GetCartHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser) : IRequestHandler<GetCartQuery, CartResponse>
{
    public async Task<CartResponse> Handle(GetCartQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId!.Value;

        var cart = await context.Carts
            .AsNoTracking()
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Product)
            .ThenInclude(p => p.ProductImages)
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

        // Queries must not modify state (ADR-002) — an empty response is returned for a user
        // who has never added anything to their cart, rather than persisting a Cart row here.
        // AddCartItemHandler is responsible for lazily creating the Cart row on first write.
        if (cart is null)
            return new CartResponse(Guid.Empty, [], 0m);

        return cart.ToResponse();
    }
}
