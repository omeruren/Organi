using MediatR;
using Organi.Server.Application.Features.Orders.DTOs;

namespace Organi.Server.Application.Features.Orders.Commands.CancelOrder;

public sealed record CancelOrderCommand(string? Reason) : IRequest<OrderResponse>
{
    public Guid Id { get; init; }
}
