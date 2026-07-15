namespace Organi.Server.Application.Features.Reports.DTOs;

public sealed record SalesReportResponse(
    decimal TotalRevenue,
    int TotalOrders,
    decimal AverageOrderValue,
    DateTime? FromDate,
    DateTime? ToDate);
