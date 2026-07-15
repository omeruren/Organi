using MediatR;
using Microsoft.EntityFrameworkCore;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Reports.DTOs;
using Organi.Server.Domain.Enums;

namespace Organi.Server.Application.Features.Reports.Queries.GetVendorsReport;

public sealed class GetVendorsReportHandler(
    IApplicationDbContext context) : IRequestHandler<GetVendorsReportQuery, IReadOnlyList<VendorReportItemResponse>>
{
    private static readonly OrderStatus[] QualifyingStatuses =
        [OrderStatus.Confirmed, OrderStatus.Processing, OrderStatus.Shipped, OrderStatus.Delivered];

    public async Task<IReadOnlyList<VendorReportItemResponse>> Handle(GetVendorsReportQuery request, CancellationToken cancellationToken)
    {
        // EF Core cannot translate Distinct().Count() nested inside a GroupBy/Sum projection,
        // so the qualifying rows are fetched flat and the aggregation runs client-side (LINQ to Objects).
        var rows = await context.OrderItems
            .AsNoTracking()
            .Where(oi => QualifyingStatuses.Contains(oi.Order.Status))
            .Select(oi => new { oi.VendorId, oi.Vendor.StoreName, oi.OrderId, oi.TotalPrice })
            .ToListAsync(cancellationToken);

        return rows
            .GroupBy(r => new { r.VendorId, r.StoreName })
            .Select(g => new VendorReportItemResponse(
                g.Key.VendorId,
                g.Key.StoreName,
                g.Sum(r => r.TotalPrice),
                g.Select(r => r.OrderId).Distinct().Count()))
            .OrderByDescending(v => v.TotalRevenue)
            .Take(request.Top)
            .ToList();
    }
}
