using Organi.Server.Domain.Common;

namespace Organi.Server.Domain.Entities;

public sealed class OrderItem : BaseEntity
{
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public string ProductName { get; set; } = default!;
    public string ProductSKU { get; set; } = default!;
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public Guid VendorId { get; set; }

    public Order Order { get; set; } = default!;
    public Product Product { get; set; } = default!;
    public Vendor Vendor { get; set; } = default!;
}
