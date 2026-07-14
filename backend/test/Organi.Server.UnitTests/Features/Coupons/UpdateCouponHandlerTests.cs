using FluentAssertions;
using Microsoft.Extensions.Logging;
using MockQueryable.NSubstitute;
using NSubstitute;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Coupons.Commands.UpdateCoupon;
using Organi.Server.Domain.Entities;
using Organi.Server.Domain.Enums;
using Organi.Server.Domain.Exceptions;
using Xunit;

namespace Organi.Server.UnitTests.Features.Coupons;

public sealed class UpdateCouponHandlerTests
{
    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly ILogger<UpdateCouponHandler> _logger = Substitute.For<ILogger<UpdateCouponHandler>>();
    private readonly UpdateCouponHandler _handler;

    public UpdateCouponHandlerTests()
    {
        _handler = new UpdateCouponHandler(_context, _logger);
    }

    private void SetupCoupons(params Coupon[] coupons)
    {
        var mockSet = coupons.ToList().BuildMockDbSet();
        _context.Coupons.Returns(mockSet);
    }

    private static UpdateCouponCommand ValidCommand(Guid id) => new(
        "SAVE20", "Updated", "Percentage", 20m, null, null,
        DateTime.UtcNow, DateTime.UtcNow.AddDays(30), true)
    { Id = id };

    [Fact]
    public async Task Handle_CouponNotFound_ThrowsNotFoundException()
    {
        SetupCoupons();

        var act = () => _handler.Handle(ValidCommand(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_DuplicateCodeFromAnotherCoupon_ThrowsBusinessRuleException()
    {
        var couponId = Guid.NewGuid();
        var coupon = new Coupon { Id = couponId, Code = "SAVE10", DiscountType = DiscountType.Percentage, DiscountValue = 10m };
        var other = new Coupon { Id = Guid.NewGuid(), Code = "SAVE20" };

        SetupCoupons(coupon, other);

        var act = () => _handler.Handle(ValidCommand(couponId), CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*already exists*");
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsUpdatedCouponResponse()
    {
        var couponId = Guid.NewGuid();
        var coupon = new Coupon { Id = couponId, Code = "SAVE10", DiscountType = DiscountType.Percentage, DiscountValue = 10m };

        SetupCoupons(coupon);

        var result = await _handler.Handle(ValidCommand(couponId), CancellationToken.None);

        result.Code.Should().Be("SAVE20");
        result.DiscountValue.Should().Be(20m);
    }
}
