using MediatR;
using Organi.Server.Application.Features.Orders.DTOs;

namespace Organi.Server.Application.Features.Orders.Commands.ShipOrder;

public sealed record ShipOrderCommand(Guid Id) : IRequest<OrderResponse>;
