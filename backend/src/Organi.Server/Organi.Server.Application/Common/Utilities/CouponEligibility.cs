using Organi.Server.Domain.Entities;
using Organi.Server.Domain.Enums;
using Organi.Server.Domain.Exceptions;

namespace Organi.Server.Application.Common.Utilities;

public static class CouponEligibility
{
    public static void EnsureEligible(Coupon coupon, decimal subtotal)
    {
        if (!coupon.IsActive)
            throw new BusinessRuleException("This coupon is no longer active.");

        var now = DateTime.UtcNow;
        if (now < coupon.StartDate || now > coupon.EndDate)
            throw new BusinessRuleException("This coupon is not currently valid.");

        if (coupon.MaxUsageCount.HasValue && coupon.CurrentUsageCount >= coupon.MaxUsageCount.Value)
            throw new BusinessRuleException("This coupon has reached its usage limit.");

        if (coupon.MinimumOrderAmount.HasValue && subtotal < coupon.MinimumOrderAmount.Value)
            throw new BusinessRuleException(
                $"This coupon requires a minimum order amount of {coupon.MinimumOrderAmount.Value:F2}.");
    }

    public static decimal CalculateDiscount(Coupon coupon, decimal subtotal) => coupon.DiscountType switch
    {
        DiscountType.Percentage => subtotal * (coupon.DiscountValue / 100m),
        DiscountType.FixedAmount => Math.Min(coupon.DiscountValue, subtotal),
        _ => 0m
    };
}
