using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Orders.DTOs;
using Organi.Server.Domain.Entities;

namespace Organi.Server.Application.Features.Orders.Mappings;

public static class OrderMappingExtensions
{
    /// <summary>
    /// Snapshots the order into a plain model for the email layer. Called before the response is
    /// returned, while the entity is still loaded — the email service never touches the DbContext.
    /// Notifications go to the shipping email captured on the order, not the account email.
    /// </summary>
    public static OrderEmailModel ToEmailModel(this Order order) => new(
        order.OrderNumber,
        order.Status.ToString(),
        order.ShippingEmail,
        $"{order.ShippingFirstName} {order.ShippingLastName}",
        order.SubTotal,
        order.DiscountAmount,
        order.ShippingCost,
        order.TaxAmount,
        order.TotalAmount,
        order.OrderItems
            .Select(oi => new OrderEmailLine(oi.ProductName, oi.Quantity, oi.UnitPrice, oi.TotalPrice))
            .ToList(),
        order.CancellationReason);

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
