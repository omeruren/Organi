using MediatR;
using Microsoft.EntityFrameworkCore;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Orders.DTOs;
using Organi.Server.Application.Features.Orders.Mappings;
using Organi.Server.Domain.Exceptions;

namespace Organi.Server.Application.Features.Orders.Queries.GetOrderById;

public sealed class GetOrderByIdHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser) : IRequestHandler<GetOrderByIdQuery, OrderResponse>
{
    public async Task<OrderResponse> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await context.Orders
            .AsNoTracking()
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Order", request.Id);

        var isOwner = order.UserId == currentUser.UserId;
        var isParticipatingVendor = currentUser.VendorId.HasValue
            && order.OrderItems.Any(oi => oi.VendorId == currentUser.VendorId);

        if (!currentUser.IsInRole("Admin") && !isOwner && !isParticipatingVendor)
            throw new NotFoundException("Order", request.Id);

        return order.ToResponse();
    }
}
