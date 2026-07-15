using MediatR;
using Organi.Server.Application.Features.Wishlist.DTOs;

namespace Organi.Server.Application.Features.Wishlist.Commands.AddWishlistItem;

public sealed record AddWishlistItemCommand(Guid ProductId) : IRequest<WishlistItemResponse>;
