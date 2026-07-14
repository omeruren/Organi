using MediatR;
using Organi.Server.Application.Features.Cart.DTOs;

namespace Organi.Server.Application.Features.Cart.Commands.AddCartItem;

public sealed record AddCartItemCommand(Guid ProductId, int Quantity) : IRequest<CartResponse>;
