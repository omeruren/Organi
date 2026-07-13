using Organi.Server.Domain.Common;

namespace Organi.Server.Domain.Entities;

public sealed class CartItem : BaseEntity
{
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public Guid CartId { get; set; }
    public Guid ProductId { get; set; }

    public Cart Cart { get; set; } = default!;
    public Product Product { get; set; } = default!;
}
