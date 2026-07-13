using Organi.Server.Domain.Common;
using Organi.Server.Domain.Enums;

namespace Organi.Server.Domain.Entities;

public sealed class Vendor : AuditableEntity
{
    public string StoreName { get; set; } = default!;
    public string Slug { get; set; } = default!;
    public string? Description { get; set; }
    public string? LogoUrl { get; set; }
    public string? BannerUrl { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public decimal Rating { get; set; }
    public int TotalSales { get; set; }
    public int FollowerCount { get; set; }
    public VendorStatus Status { get; set; } = VendorStatus.Pending;
    public Guid UserId { get; set; }

    public User User { get; set; } = default!;
    public ICollection<Product> Products { get; set; } = [];
}
