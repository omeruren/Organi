using Organi.Server.Domain.Common;
using Organi.Server.Domain.Enums;

namespace Organi.Server.Domain.Entities;

public sealed class Product : AuditableEntity
{
    public string Name { get; set; } = default!;
    public string Slug { get; set; } = default!;
    public string? Description { get; set; }
    public string? ShortDescription { get; set; }
    public decimal Price { get; set; }
    public decimal? SalePrice { get; set; }
    public string SKU { get; set; } = default!;
    public int StockQuantity { get; set; }
    public string Unit { get; set; } = default!;
    public decimal? Weight { get; set; }
    public bool IsOrganic { get; set; }
    public bool IsFeatured { get; set; }
    public ProductStatus Status { get; set; } = ProductStatus.Draft;
    public decimal AverageRating { get; set; }
    public int ReviewCount { get; set; }
    public Guid CategoryId { get; set; }
    public Guid VendorId { get; set; }

    public Category Category { get; set; } = default!;
    public Vendor Vendor { get; set; } = default!;
    public ICollection<ProductImage> ProductImages { get; set; } = [];
    public ICollection<OrderItem> OrderItems { get; set; } = [];
    public ICollection<Review> Reviews { get; set; } = [];
    public ICollection<WishlistItem> WishlistItems { get; set; } = [];
    public ICollection<CompareItem> CompareItems { get; set; } = [];
    public ICollection<CartItem> CartItems { get; set; } = [];
}
