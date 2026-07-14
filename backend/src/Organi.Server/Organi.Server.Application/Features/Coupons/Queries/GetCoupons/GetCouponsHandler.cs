using MediatR;
using Microsoft.EntityFrameworkCore;
using Organi.Server.Application.Common.Extensions;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Common.Models;
using Organi.Server.Application.Features.Coupons.DTOs;

namespace Organi.Server.Application.Features.Coupons.Queries.GetCoupons;

public sealed class GetCouponsHandler(
    IApplicationDbContext context) : IRequestHandler<GetCouponsQuery, PagedResponse<CouponResponse>>
{
    public async Task<PagedResponse<CouponResponse>> Handle(GetCouponsQuery request, CancellationToken cancellationToken)
    {
        var projected = context.Coupons
            .AsNoTracking()
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new CouponResponse(
                c.Id,
                c.Code,
                c.Description,
                c.DiscountType.ToString(),
                c.DiscountValue,
                c.MinimumOrderAmount,
                c.MaxUsageCount,
                c.CurrentUsageCount,
                c.StartDate,
                c.EndDate,
                c.IsActive,
                c.CreatedAt,
                c.UpdatedAt));

        return await projected.ToPagedResponseAsync(request.Page, request.PageSize, cancellationToken);
    }
}
