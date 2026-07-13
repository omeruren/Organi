using Organi.Server.Domain.Common;
using Organi.Server.Domain.Enums;

namespace Organi.Server.Domain.Entities;

public sealed class Coupon : AuditableEntity
{
    public string Code { get; set; } = default!;
    public string? Description { get; set; }
    public DiscountType DiscountType { get; set; }
    public decimal DiscountValue { get; set; }
    public decimal? MinimumOrderAmount { get; set; }
    public int? MaxUsageCount { get; set; }
    public int CurrentUsageCount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<Order> Orders { get; set; } = [];
}
