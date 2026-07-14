namespace Organi.Server.Application.Features.Orders.DTOs;

public sealed record OrderItemResponse(
    Guid Id,
    Guid ProductId,
    string ProductName,
    string ProductSKU,
    Guid VendorId,
    int Quantity,
    decimal UnitPrice,
    decimal TotalPrice);
