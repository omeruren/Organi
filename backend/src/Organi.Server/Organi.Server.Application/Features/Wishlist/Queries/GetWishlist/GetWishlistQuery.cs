using MediatR;
using Organi.Server.Application.Features.Wishlist.DTOs;

namespace Organi.Server.Application.Features.Wishlist.Queries.GetWishlist;

public sealed record GetWishlistQuery : IRequest<IReadOnlyList<WishlistItemResponse>>;
