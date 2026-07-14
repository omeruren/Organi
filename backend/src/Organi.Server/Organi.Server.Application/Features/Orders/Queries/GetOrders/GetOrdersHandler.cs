using MediatR;
using Microsoft.EntityFrameworkCore;
using Organi.Server.Application.Common.Extensions;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Common.Models;
using Organi.Server.Application.Features.Orders.DTOs;

namespace Organi.Server.Application.Features.Orders.Queries.GetOrders;

public sealed class GetOrdersHandler(
    IApplicationDbContext context) : IRequestHandler<GetOrdersQuery, PagedResponse<OrderSummaryResponse>>
{
    public async Task<PagedResponse<OrderSummaryResponse>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var projected = context.Orders
            .AsNoTracking()
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
