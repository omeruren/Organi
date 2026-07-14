namespace Organi.Server.Application.Features.Orders.DTOs;

public sealed record OrderSummaryResponse(
    Guid Id,
    string OrderNumber,
    decimal TotalAmount,
    string Status,
    int ItemCount,
    DateTime CreatedAt);
