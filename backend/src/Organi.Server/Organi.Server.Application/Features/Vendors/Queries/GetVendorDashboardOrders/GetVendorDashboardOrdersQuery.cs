using MediatR;
using Organi.Server.Application.Common.Models;
using Organi.Server.Application.Features.Orders.DTOs;

namespace Organi.Server.Application.Features.Vendors.Queries.GetVendorDashboardOrders;

public sealed record GetVendorDashboardOrdersQuery(
    string? Status = null,
    string? Search = null,
    int Page = 1,
    int PageSize = 10) : IRequest<PagedResponse<OrderSummaryResponse>>;
