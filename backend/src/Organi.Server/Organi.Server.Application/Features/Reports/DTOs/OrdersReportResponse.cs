namespace Organi.Server.Application.Features.Reports.DTOs;

public sealed record OrdersReportResponse(
    int TotalOrders,
    int Pending,
    int Confirmed,
    int Processing,
    int Shipped,
    int Delivered,
    int Cancelled,
    int Refunded);
