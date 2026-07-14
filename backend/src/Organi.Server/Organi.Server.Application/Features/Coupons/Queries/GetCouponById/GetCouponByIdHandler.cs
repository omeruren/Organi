using MediatR;
using Microsoft.EntityFrameworkCore;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Coupons.DTOs;
using Organi.Server.Application.Features.Coupons.Mappings;
using Organi.Server.Domain.Exceptions;

namespace Organi.Server.Application.Features.Coupons.Queries.GetCouponById;

public sealed class GetCouponByIdHandler(
    IApplicationDbContext context) : IRequestHandler<GetCouponByIdQuery, CouponResponse>
{
    public async Task<CouponResponse> Handle(GetCouponByIdQuery request, CancellationToken cancellationToken)
    {
        var coupon = await context.Coupons
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Coupon", request.Id);

        return coupon.ToResponse();
    }
}
