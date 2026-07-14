using MediatR;
using Organi.Server.Application.Features.Orders.DTOs;

namespace Organi.Server.Application.Features.Orders.Commands.ConfirmOrder;

public sealed record ConfirmOrderCommand(Guid Id) : IRequest<OrderResponse>;
