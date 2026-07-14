using MediatR;
using Organi.Server.Application.Features.Cart.DTOs;

namespace Organi.Server.Application.Features.Cart.Queries.GetCart;

public sealed record GetCartQuery : IRequest<CartResponse>;
