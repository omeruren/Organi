using MediatR;
using Microsoft.EntityFrameworkCore;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Reports.DTOs;
using Organi.Server.Domain.Enums;

namespace Organi.Server.Application.Features.Reports.Queries.GetOrdersReport;

public sealed class GetOrdersReportHandler(
    IApplicationDbContext context) : IRequestHandler<GetOrdersReportQuery, OrdersReportResponse>
{
    public async Task<OrdersReportResponse> Handle(GetOrdersReportQuery request, CancellationToken cancellationToken)
    {
        var counts = await context.Orders
            .AsNoTracking()
            .GroupBy(o => o.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        int CountOf(OrderStatus status) => counts.FirstOrDefault(c => c.Status == status)?.Count ?? 0;

        return new OrdersReportResponse(
            counts.Sum(c => c.Count),
            CountOf(OrderStatus.Pending),
            CountOf(OrderStatus.Confirmed),
            CountOf(OrderStatus.Processing),
            CountOf(OrderStatus.Shipped),
            CountOf(OrderStatus.Delivered),
            CountOf(OrderStatus.Cancelled),
            CountOf(OrderStatus.Refunded));
    }
}
