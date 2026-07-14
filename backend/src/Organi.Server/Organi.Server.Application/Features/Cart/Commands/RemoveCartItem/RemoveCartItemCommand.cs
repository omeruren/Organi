using MediatR;

namespace Organi.Server.Application.Features.Cart.Commands.RemoveCartItem;

public sealed record RemoveCartItemCommand(Guid Id) : IRequest;
