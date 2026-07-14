using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Coupons.DTOs;
using Organi.Server.Application.Features.Coupons.Mappings;
using Organi.Server.Domain.Entities;
using Organi.Server.Domain.Enums;
using Organi.Server.Domain.Exceptions;

namespace Organi.Server.Application.Features.Coupons.Commands.CreateCoupon;

public sealed class CreateCouponHandler(
    IApplicationDbContext context,
    ILogger<CreateCouponHandler> logger) : IRequestHandler<CreateCouponCommand, CouponResponse>
{
    public async Task<CouponResponse> Handle(CreateCouponCommand request, CancellationToken cancellationToken)
    {
        var codeExists = await context.Coupons.AnyAsync(c => c.Code == request.Code, cancellationToken);
        if (codeExists)
            throw new BusinessRuleException($"A coupon with code '{request.Code}' already exists.");

        var coupon = new Coupon
        {
            Code = request.Code,
            Description = request.Description,
            DiscountType = Enum.Parse<DiscountType>(request.DiscountType, ignoreCase: true),
            DiscountValue = request.DiscountValue,
            MinimumOrderAmount = request.MinimumOrderAmount,
            MaxUsageCount = request.MaxUsageCount,
            CurrentUsageCount = 0,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            IsActive = true
        };

        context.Coupons.Add(coupon);
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Coupon {CouponId} created with code {Code}", coupon.Id, coupon.Code);

        return coupon.ToResponse();
    }
}
