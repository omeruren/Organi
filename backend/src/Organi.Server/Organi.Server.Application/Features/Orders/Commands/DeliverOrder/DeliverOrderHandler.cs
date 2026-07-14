using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Orders.DTOs;
using Organi.Server.Application.Features.Orders.Mappings;
using Organi.Server.Domain.Enums;
using Organi.Server.Domain.Exceptions;

namespace Organi.Server.Application.Features.Orders.Commands.DeliverOrder;

public sealed class DeliverOrderHandler(
    IApplicationDbContext context,
    ILogger<DeliverOrderHandler> logger) : IRequestHandler<DeliverOrderCommand, OrderResponse>
{
    public async Task<OrderResponse> Handle(DeliverOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Order", request.Id);

        if (order.Status != OrderStatus.Shipped)
            throw new BusinessRuleException("Only shipped orders can be marked as delivered.");

        order.Status = OrderStatus.Delivered;
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Order {OrderId} delivered", order.Id);

        return order.ToResponse();
    }
}
