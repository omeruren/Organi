using MediatR;
using Microsoft.EntityFrameworkCore;
using Organi.Server.Application.Common.Extensions;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Common.Models;
using Organi.Server.Application.Features.Orders.DTOs;
using Organi.Server.Domain.Enums;
using Organi.Server.Domain.Exceptions;

namespace Organi.Server.Application.Features.Vendors.Queries.GetVendorDashboardOrders;

public sealed class GetVendorDashboardOrdersHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser) : IRequestHandler<GetVendorDashboardOrdersQuery, PagedResponse<OrderSummaryResponse>>
{
    public async Task<PagedResponse<OrderSummaryResponse>> Handle(GetVendorDashboardOrdersQuery request, CancellationToken cancellationToken)
    {
        var vendorId = currentUser.VendorId
            ?? throw new BusinessRuleException("You do not have a vendor profile.");

        // Orders containing at least one of the vendor's items; TotalAmount and
        // ItemCount are whole-order values, not the vendor's slice.
        var query = context.Orders
            .AsNoTracking()
            .Where(o => o.OrderItems.Any(oi => oi.VendorId == vendorId));

        if (request.Status is not null)
        {
            var status = Enum.Parse<OrderStatus>(request.Status, ignoreCase: true);
            query = query.Where(o => o.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
            query = query.Where(o => o.OrderNumber.Contains(request.Search));

        var projected = query
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new OrderSummaryResponse(
                o.Id,
                o.OrderNumber,
                o.User.FirstName + " " + o.User.LastName,
                o.TotalAmount,
                o.Status.ToString(),
                o.OrderItems.Count,
                o.CreatedAt));

        return await projected.ToPagedResponseAsync(request.Page, request.PageSize, cancellationToken);
    }
}
