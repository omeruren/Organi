using MediatR;
using Organi.Server.Application.Features.Reports.DTOs;

namespace Organi.Server.Application.Features.Reports.Queries.GetOrdersReport;

public sealed record GetOrdersReportQuery : IRequest<OrdersReportResponse>;
