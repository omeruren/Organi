using MediatR;
using Microsoft.EntityFrameworkCore;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Common.Utilities;
using Organi.Server.Application.Features.Coupons.DTOs;
using Organi.Server.Domain.Exceptions;

namespace Organi.Server.Application.Features.Coupons.Queries.ValidateCoupon;

public sealed class ValidateCouponHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser) : IRequestHandler<ValidateCouponQuery, CouponValidationResponse>
{
    public async Task<CouponValidationResponse> Handle(ValidateCouponQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId!.Value;

        var cart = await context.Carts
            .AsNoTracking()
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

        if (cart is null || cart.CartItems.Count == 0)
            throw new BusinessRuleException("Your cart is empty.");

        var subTotal = cart.CartItems.Sum(ci => ci.UnitPrice * ci.Quantity);

        var coupon = await context.Coupons
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Code == request.Code, cancellationToken)
            ?? throw new BusinessRuleException("Invalid coupon code.");

        CouponEligibility.EnsureEligible(coupon, subTotal);

        var discountAmount = CouponEligibility.CalculateDiscount(coupon, subTotal);

        return new CouponValidationResponse(
            coupon.Id,
            coupon.Code,
            coupon.DiscountType.ToString(),
            coupon.DiscountValue,
            subTotal,
            discountAmount,
            subTotal - discountAmount);
    }
}
