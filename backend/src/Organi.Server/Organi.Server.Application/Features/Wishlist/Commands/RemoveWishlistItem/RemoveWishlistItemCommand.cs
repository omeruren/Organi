using MediatR;

namespace Organi.Server.Application.Features.Wishlist.Commands.RemoveWishlistItem;

public sealed record RemoveWishlistItemCommand(Guid ProductId) : IRequest;
