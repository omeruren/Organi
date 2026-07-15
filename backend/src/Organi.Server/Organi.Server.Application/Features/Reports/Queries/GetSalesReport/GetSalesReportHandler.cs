using MediatR;
using Microsoft.EntityFrameworkCore;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Reports.DTOs;
using Organi.Server.Domain.Enums;

namespace Organi.Server.Application.Features.Reports.Queries.GetSalesReport;

public sealed class GetSalesReportHandler(
    IApplicationDbContext context) : IRequestHandler<GetSalesReportQuery, SalesReportResponse>
{
    private static readonly OrderStatus[] QualifyingStatuses =
        [OrderStatus.Confirmed, OrderStatus.Processing, OrderStatus.Shipped, OrderStatus.Delivered];

    public async Task<SalesReportResponse> Handle(GetSalesReportQuery request, CancellationToken cancellationToken)
    {
        var query = context.Orders
            .AsNoTracking()
            .Where(o => QualifyingStatuses.Contains(o.Status));

        if (request.FromDate.HasValue)
            query = query.Where(o => o.CreatedAt >= request.FromDate.Value);

        if (request.ToDate.HasValue)
            query = query.Where(o => o.CreatedAt <= request.ToDate.Value);

        var stats = await query
            .GroupBy(_ => 1)
            .Select(g => new { Revenue = g.Sum(o => o.TotalAmount), Count = g.Count() })
            .FirstOrDefaultAsync(cancellationToken);

        var totalRevenue = stats?.Revenue ?? 0m;
        var totalOrders = stats?.Count ?? 0;
        var averageOrderValue = totalOrders > 0 ? totalRevenue / totalOrders : 0m;

        return new SalesReportResponse(totalRevenue, totalOrders, averageOrderValue, request.FromDate, request.ToDate);
    }
}
