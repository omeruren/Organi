using MediatR;
using Organi.Server.Application.Features.Orders.DTOs;

namespace Organi.Server.Application.Features.Orders.Commands.CreateOrder;

public sealed record CreateOrderCommand(
    string ShippingFirstName,
    string ShippingLastName,
    string ShippingAddress,
    string ShippingCity,
    string? ShippingPostalCode,
    string ShippingPhone,
    string ShippingEmail,
    string? Notes,
    string? CouponCode) : IRequest<OrderResponse>;
