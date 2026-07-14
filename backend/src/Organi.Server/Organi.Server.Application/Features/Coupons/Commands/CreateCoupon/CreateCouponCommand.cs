using MediatR;
using Organi.Server.Application.Features.Coupons.DTOs;

namespace Organi.Server.Application.Features.Coupons.Commands.CreateCoupon;

public sealed record CreateCouponCommand(
    string Code,
    string? Description,
    string DiscountType,
    decimal DiscountValue,
    decimal? MinimumOrderAmount,
    int? MaxUsageCount,
    DateTime StartDate,
    DateTime EndDate) : IRequest<CouponResponse>;
