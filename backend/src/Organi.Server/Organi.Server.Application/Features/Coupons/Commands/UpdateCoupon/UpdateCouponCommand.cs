using MediatR;
using Organi.Server.Application.Features.Coupons.DTOs;

namespace Organi.Server.Application.Features.Coupons.Commands.UpdateCoupon;

public sealed record UpdateCouponCommand(
    string Code,
    string? Description,
    string DiscountType,
    decimal DiscountValue,
    decimal? MinimumOrderAmount,
    int? MaxUsageCount,
    DateTime StartDate,
    DateTime EndDate,
    bool IsActive) : IRequest<CouponResponse>
{
    public Guid Id { get; init; }
}
