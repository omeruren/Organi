using MediatR;
using Microsoft.EntityFrameworkCore;
using Organi.Server.Application.Common.Extensions;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Common.Models;
using Organi.Server.Application.Features.Orders.DTOs;

namespace Organi.Server.Application.Features.Orders.Queries.GetMyOrders;

public sealed class GetMyOrdersHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser) : IRequestHandler<GetMyOrdersQuery, PagedResponse<OrderSummaryResponse>>
{
    public async Task<PagedResponse<OrderSummaryResponse>> Handle(GetMyOrdersQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId!.Value;

        var projected = context.Orders
            .AsNoTracking()
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new OrderSummaryResponse(
                o.Id,
                o.OrderNumber,
                o.TotalAmount,
                o.Status.ToString(),
                o.OrderItems.Count,
                o.CreatedAt));

        return await projected.ToPagedResponseAsync(request.Page, request.PageSize, cancellationToken);
    }
}
