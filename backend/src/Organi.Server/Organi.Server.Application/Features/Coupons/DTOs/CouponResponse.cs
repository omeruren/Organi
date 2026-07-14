namespace Organi.Server.Application.Features.Coupons.DTOs;

public sealed record CouponResponse(
    Guid Id,
    string Code,
    string? Description,
    string DiscountType,
    decimal DiscountValue,
    decimal? MinimumOrderAmount,
    int? MaxUsageCount,
    int CurrentUsageCount,
    DateTime StartDate,
    DateTime EndDate,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
