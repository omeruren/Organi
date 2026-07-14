using MediatR;
using Organi.Server.Application.Common.Models;
using Organi.Server.Application.Features.Orders.DTOs;

namespace Organi.Server.Application.Features.Orders.Queries.GetOrders;

public sealed record GetOrdersQuery(int Page = 1, int PageSize = 10) : IRequest<PagedResponse<OrderSummaryResponse>>;
