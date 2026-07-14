using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Coupons.DTOs;
using Organi.Server.Application.Features.Coupons.Mappings;
using Organi.Server.Domain.Enums;
using Organi.Server.Domain.Exceptions;

namespace Organi.Server.Application.Features.Coupons.Commands.UpdateCoupon;

public sealed class UpdateCouponHandler(
    IApplicationDbContext context,
    ILogger<UpdateCouponHandler> logger) : IRequestHandler<UpdateCouponCommand, CouponResponse>
{
    public async Task<CouponResponse> Handle(UpdateCouponCommand request, CancellationToken cancellationToken)
    {
        var coupon = await context.Coupons.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Coupon", request.Id);

        var codeExists = await context.Coupons
            .AnyAsync(c => c.Code == request.Code && c.Id != coupon.Id, cancellationToken);
        if (codeExists)
            throw new BusinessRuleException($"A coupon with code '{request.Code}' already exists.");

        coupon.Code = request.Code;
        coupon.Description = request.Description;
        coupon.DiscountType = Enum.Parse<DiscountType>(request.DiscountType, ignoreCase: true);
        coupon.DiscountValue = request.DiscountValue;
        coupon.MinimumOrderAmount = request.MinimumOrderAmount;
        coupon.MaxUsageCount = request.MaxUsageCount;
        coupon.StartDate = request.StartDate;
        coupon.EndDate = request.EndDate;
        coupon.IsActive = request.IsActive;

        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Coupon {CouponId} updated", coupon.Id);

        return coupon.ToResponse();
    }
}
