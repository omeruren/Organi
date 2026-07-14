using FluentAssertions;
using MockQueryable.NSubstitute;
using NSubstitute;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Coupons.Queries.ValidateCoupon;
using Organi.Server.Domain.Entities;
using Organi.Server.Domain.Enums;
using Organi.Server.Domain.Exceptions;
using Xunit;
using CartEntity = Organi.Server.Domain.Entities.Cart;

namespace Organi.Server.UnitTests.Features.Coupons;

public sealed class ValidateCouponHandlerTests
{
    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
    private readonly ValidateCouponHandler _handler;

    public ValidateCouponHandlerTests()
    {
        _handler = new ValidateCouponHandler(_context, _currentUser);
    }

    private void SetupCarts(params CartEntity[] carts)
    {
        var mockSet = carts.ToList().BuildMockDbSet();
        _context.Carts.Returns(mockSet);
    }

    private void SetupCoupons(params Coupon[] coupons)
    {
        var mockSet = coupons.ToList().BuildMockDbSet();
        _context.Coupons.Returns(mockSet);
    }

    private static CartEntity CartWithSubtotal(Guid userId, decimal unitPrice, int quantity) => new()
    {
        UserId = userId,
        CartItems = [new CartItem { Quantity = quantity, UnitPrice = unitPrice }]
    };

    [Fact]
    public async Task Handle_EmptyCart_ThrowsBusinessRuleException()
    {
        _currentUser.UserId.Returns(Guid.NewGuid());
        SetupCarts();

        var act = () => _handler.Handle(new ValidateCouponQuery("SAVE10"), CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*cart is empty*");
    }

    [Fact]
    public async Task Handle_UnknownCode_ThrowsBusinessRuleException()
    {
        var userId = Guid.NewGuid();
        _currentUser.UserId.Returns(userId);
        SetupCarts(CartWithSubtotal(userId, 10m, 2));
        SetupCoupons();

        var act = () => _handler.Handle(new ValidateCouponQuery("MISSING"), CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*Invalid coupon*");
    }

    [Fact]
    public async Task Handle_BelowMinimumOrderAmount_ThrowsBusinessRuleException()
    {
        var userId = Guid.NewGuid();
        _currentUser.UserId.Returns(userId);
        SetupCarts(CartWithSubtotal(userId, 10m, 1));
        SetupCoupons(new Coupon
        {
            Code = "BIG50",
            DiscountType = DiscountType.FixedAmount,
            DiscountValue = 5m,
            MinimumOrderAmount = 50m,
            IsActive = true,
            StartDate = DateTime.UtcNow.AddDays(-1),
            EndDate = DateTime.UtcNow.AddDays(1)
        });

        var act = () => _handler.Handle(new ValidateCouponQuery("BIG50"), CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*minimum order amount*");
    }

    [Fact]
    public async Task Handle_UsageLimitReached_ThrowsBusinessRuleException()
    {
        var userId = Guid.NewGuid();
        _currentUser.UserId.Returns(userId);
        SetupCarts(CartWithSubtotal(userId, 10m, 2));
        SetupCoupons(new Coupon
        {
            Code = "LIMITED",
            DiscountType = DiscountType.FixedAmount,
            DiscountValue = 5m,
            MaxUsageCount = 1,
            CurrentUsageCount = 1,
            IsActive = true,
            StartDate = DateTime.UtcNow.AddDays(-1),
            EndDate = DateTime.UtcNow.AddDays(1)
        });

        var act = () => _handler.Handle(new ValidateCouponQuery("LIMITED"), CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*usage limit*");
    }

    [Fact]
    public async Task Handle_ValidPercentageCoupon_ReturnsCorrectDiscount()
    {
        var userId = Guid.NewGuid();
        _currentUser.UserId.Returns(userId);
        SetupCarts(CartWithSubtotal(userId, 10m, 2));
        SetupCoupons(new Coupon
        {
            Code = "SAVE10",
            DiscountType = DiscountType.Percentage,
            DiscountValue = 10m,
            IsActive = true,
            StartDate = DateTime.UtcNow.AddDays(-1),
            EndDate = DateTime.UtcNow.AddDays(1)
        });

        var result = await _handler.Handle(new ValidateCouponQuery("SAVE10"), CancellationToken.None);

        result.SubTotal.Should().Be(20m);
        result.DiscountAmount.Should().Be(2m);
        result.TotalAfterDiscount.Should().Be(18m);
    }
}
