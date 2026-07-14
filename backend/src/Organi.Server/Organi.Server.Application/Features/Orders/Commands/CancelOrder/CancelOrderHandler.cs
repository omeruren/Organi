using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Orders.DTOs;
using Organi.Server.Application.Features.Orders.Mappings;
using Organi.Server.Domain.Enums;
using Organi.Server.Domain.Exceptions;

namespace Organi.Server.Application.Features.Orders.Commands.CancelOrder;

public sealed class CancelOrderHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser,
    ILogger<CancelOrderHandler> logger) : IRequestHandler<CancelOrderCommand, OrderResponse>
{
    private static readonly OrderStatus[] StockCommittedStatuses = [OrderStatus.Confirmed, OrderStatus.Shipped];

    public async Task<OrderResponse> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Order", request.Id);

        if (!currentUser.IsInRole("Admin") && order.UserId != currentUser.UserId)
            throw new ForbiddenException("You do not have permission to cancel this order.");

        if (order.Status == OrderStatus.Delivered)
            throw new BusinessRuleException("Delivered orders cannot be cancelled.");

        if (order.Status == OrderStatus.Cancelled)
            throw new BusinessRuleException("Order is already cancelled.");

        if (StockCommittedStatuses.Contains(order.Status))
        {
            foreach (var item in order.OrderItems)
            {
                item.Product.StockQuantity += item.Quantity;
            }
        }

        order.Status = OrderStatus.Cancelled;
        order.CancellationReason = request.Reason;
        order.CancelledAt = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Order {OrderId} cancelled by user {UserId}", order.Id, currentUser.UserId);

        return order.ToResponse();
    }
}
