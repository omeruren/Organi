using Organi.Server.Application.Features.Coupons.DTOs;
using Organi.Server.Domain.Entities;

namespace Organi.Server.Application.Features.Coupons.Mappings;

public static class CouponMappingExtensions
{
    public static CouponResponse ToResponse(this Coupon coupon) => new(
        coupon.Id,
        coupon.Code,
        coupon.Description,
        coupon.DiscountType.ToString(),
        coupon.DiscountValue,
        coupon.MinimumOrderAmount,
        coupon.MaxUsageCount,
        coupon.CurrentUsageCount,
        coupon.StartDate,
        coupon.EndDate,
        coupon.IsActive,
        coupon.CreatedAt,
        coupon.UpdatedAt);
}
