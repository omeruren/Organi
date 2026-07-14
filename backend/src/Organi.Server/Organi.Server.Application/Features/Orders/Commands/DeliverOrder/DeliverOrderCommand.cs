using MediatR;
using Organi.Server.Application.Features.Orders.DTOs;

namespace Organi.Server.Application.Features.Orders.Commands.DeliverOrder;

public sealed record DeliverOrderCommand(Guid Id) : IRequest<OrderResponse>;
