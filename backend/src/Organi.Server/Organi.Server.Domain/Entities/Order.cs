using Organi.Server.Domain.Common;
using Organi.Server.Domain.Enums;

namespace Organi.Server.Domain.Entities;

public sealed class Order : AuditableEntity
{
    public string OrderNumber { get; set; } = default!;
    public decimal SubTotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public string? Notes { get; set; }
    public string? CancellationReason { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string ShippingFirstName { get; set; } = default!;
    public string ShippingLastName { get; set; } = default!;
    public string ShippingAddress { get; set; } = default!;
    public string ShippingCity { get; set; } = default!;
    public string? ShippingPostalCode { get; set; }
    public string ShippingPhone { get; set; } = default!;
    public string ShippingEmail { get; set; } = default!;
    public Guid UserId { get; set; }
    public Guid? CouponId { get; set; }

    public User User { get; set; } = default!;
    public Coupon? Coupon { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; } = [];
}
