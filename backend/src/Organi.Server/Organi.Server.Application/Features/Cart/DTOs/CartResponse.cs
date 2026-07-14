namespace Organi.Server.Application.Features.Cart.DTOs;

public sealed record CartResponse(
    Guid Id,
    IReadOnlyList<CartItemResponse> Items,
    decimal SubTotal);
