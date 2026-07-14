using MediatR;
using Organi.Server.Application.Features.Cart.DTOs;

namespace Organi.Server.Application.Features.Cart.Commands.UpdateCartItem;

public sealed record UpdateCartItemCommand(int Quantity) : IRequest<CartResponse>
{
    public Guid Id { get; init; }
}
