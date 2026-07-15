using MediatR;
using Organi.Server.Application.Features.Reports.DTOs;

namespace Organi.Server.Application.Features.Reports.Queries.GetSalesReport;

public sealed record GetSalesReportQuery(DateTime? FromDate = null, DateTime? ToDate = null)
    : IRequest<SalesReportResponse>;
