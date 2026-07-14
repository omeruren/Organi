using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Domain.Exceptions;

namespace Organi.Server.Application.Features.Coupons.Commands.DeleteCoupon;

public sealed class DeleteCouponHandler(
    IApplicationDbContext context,
    ILogger<DeleteCouponHandler> logger) : IRequestHandler<DeleteCouponCommand>
{
    public async Task Handle(DeleteCouponCommand request, CancellationToken cancellationToken)
    {
        var coupon = await context.Coupons.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Coupon", request.Id);

        context.Coupons.Remove(coupon);
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Coupon {CouponId} deleted", coupon.Id);
    }
}
