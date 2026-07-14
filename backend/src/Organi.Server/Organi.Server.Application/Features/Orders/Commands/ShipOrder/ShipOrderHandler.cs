using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Orders.DTOs;
using Organi.Server.Application.Features.Orders.Mappings;
using Organi.Server.Domain.Enums;
using Organi.Server.Domain.Exceptions;

namespace Organi.Server.Application.Features.Orders.Commands.ShipOrder;

public sealed class ShipOrderHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser,
    ILogger<ShipOrderHandler> logger) : IRequestHandler<ShipOrderCommand, OrderResponse>
{
    public async Task<OrderResponse> Handle(ShipOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Order", request.Id);

        if (!currentUser.IsInRole("Admin") && order.OrderItems.Any(oi => oi.VendorId != currentUser.VendorId))
            throw new ForbiddenException("You do not have permission to ship this order.");

        if (order.Status != OrderStatus.Confirmed)
            throw new BusinessRuleException("Only confirmed orders can be shipped.");

        order.Status = OrderStatus.Shipped;
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Order {OrderId} shipped", order.Id);

        return order.ToResponse();
    }
}
