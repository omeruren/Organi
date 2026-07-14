using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Orders.DTOs;
using Organi.Server.Application.Features.Orders.Mappings;
using Organi.Server.Domain.Enums;
using Organi.Server.Domain.Exceptions;

namespace Organi.Server.Application.Features.Orders.Commands.ConfirmOrder;

public sealed class ConfirmOrderHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser,
    ILogger<ConfirmOrderHandler> logger) : IRequestHandler<ConfirmOrderCommand, OrderResponse>
{
    public async Task<OrderResponse> Handle(ConfirmOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Order", request.Id);

        if (!currentUser.IsInRole("Admin") && order.OrderItems.Any(oi => oi.VendorId != currentUser.VendorId))
            throw new ForbiddenException("You do not have permission to confirm this order.");

        if (order.Status != OrderStatus.Pending)
            throw new BusinessRuleException("Only pending orders can be confirmed.");

        foreach (var item in order.OrderItems)
        {
            if (item.Quantity > item.Product.StockQuantity)
                throw new BusinessRuleException(
                    $"Insufficient stock for product '{item.ProductName}'. Available: {item.Product.StockQuantity}, Requested: {item.Quantity}.");
        }

        foreach (var item in order.OrderItems)
        {
            item.Product.StockQuantity -= item.Quantity;
        }

        order.Status = OrderStatus.Confirmed;
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Order {OrderId} confirmed", order.Id);

        return order.ToResponse();
    }
}
