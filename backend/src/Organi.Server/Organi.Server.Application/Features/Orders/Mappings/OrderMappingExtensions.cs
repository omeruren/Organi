using Organi.Server.Application.Features.Orders.DTOs;
using Organi.Server.Domain.Entities;

namespace Organi.Server.Application.Features.Orders.Mappings;

public static class OrderMappingExtensions
{
    public static OrderResponse ToResponse(this Order order) => new(
        order.Id,
        order.OrderNumber,
        order.SubTotal,
        order.DiscountAmount,
        order.ShippingCost,
        order.TaxAmount,
        order.TotalAmount,
        order.Status.ToString(),
        order.Notes,
        order.CancellationReason,
        order.CancelledAt,
        order.ShippingFirstName,
        order.ShippingLastName,
        order.ShippingAddress,
        order.ShippingCity,
        order.ShippingPostalCode,
        order.ShippingPhone,
        order.ShippingEmail,
        order.UserId,
        order.CouponId,
        order.OrderItems
            .Select(oi => new OrderItemResponse(
                oi.Id,
                oi.ProductId,
                oi.ProductName,
                oi.ProductSKU,
                oi.VendorId,
                oi.Quantity,
                oi.UnitPrice,
                oi.TotalPrice))
            .ToList(),
        order.CreatedAt);
}
