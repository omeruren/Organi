namespace Organi.Server.Application.Features.Coupons.DTOs;

public sealed record CouponValidationResponse(
    Guid CouponId,
    string Code,
    string DiscountType,
    decimal DiscountValue,
    decimal SubTotal,
    decimal DiscountAmount,
    decimal TotalAfterDiscount);
