using FluentAssertions;
using Microsoft.Extensions.Logging;
using MockQueryable.NSubstitute;
using NSubstitute;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Coupons.Commands.DeleteCoupon;
using Organi.Server.Domain.Entities;
using Organi.Server.Domain.Exceptions;
using Xunit;

namespace Organi.Server.UnitTests.Features.Coupons;

public sealed class DeleteCouponHandlerTests
{
    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly ILogger<DeleteCouponHandler> _logger = Substitute.For<ILogger<DeleteCouponHandler>>();
    private readonly DeleteCouponHandler _handler;

    public DeleteCouponHandlerTests()
    {
        _handler = new DeleteCouponHandler(_context, _logger);
    }

    private void SetupCoupons(params Coupon[] coupons)
    {
        var mockSet = coupons.ToList().BuildMockDbSet();
        _context.Coupons.Returns(mockSet);
    }

    [Fact]
    public async Task Handle_CouponNotFound_ThrowsNotFoundException()
    {
        SetupCoupons();

        var act = () => _handler.Handle(new DeleteCouponCommand(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_ExistingCoupon_RemovesIt()
    {
        var coupon = new Coupon { Id = Guid.NewGuid(), Code = "SAVE10" };
        SetupCoupons(coupon);

        await _handler.Handle(new DeleteCouponCommand(coupon.Id), CancellationToken.None);

        _context.Coupons.Received(1).Remove(coupon);
    }
}
