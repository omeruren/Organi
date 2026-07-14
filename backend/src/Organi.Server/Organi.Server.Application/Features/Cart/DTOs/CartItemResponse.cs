namespace Organi.Server.Application.Features.Cart.DTOs;

public sealed record CartItemResponse(
    Guid Id,
    Guid ProductId,
    string ProductName,
    string ProductSlug,
    decimal UnitPrice,
    int Quantity,
    decimal LineTotal,
    string? PrimaryImageUrl);
