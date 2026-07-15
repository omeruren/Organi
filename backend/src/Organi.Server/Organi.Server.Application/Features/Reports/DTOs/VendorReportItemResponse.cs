namespace Organi.Server.Application.Features.Reports.DTOs;

public sealed record VendorReportItemResponse(
    Guid VendorId,
    string StoreName,
    decimal TotalRevenue,
    int TotalOrders);
