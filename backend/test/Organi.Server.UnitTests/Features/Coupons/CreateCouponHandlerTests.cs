using FluentAssertions;
using Microsoft.Extensions.Logging;
using MockQueryable.NSubstitute;
using NSubstitute;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Coupons.Commands.CreateCoupon;
using Organi.Server.Domain.Entities;
using Organi.Server.Domain.Exceptions;
using Xunit;

namespace Organi.Server.UnitTests.Features.Coupons;

public sealed class CreateCouponHandlerTests
{
    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly ILogger<CreateCouponHandler> _logger = Substitute.For<ILogger<CreateCouponHandler>>();
    private readonly CreateCouponHandler _handler;

    public CreateCouponHandlerTests()
    {
        _handler = new CreateCouponHandler(_context, _logger);
    }

    private void SetupCoupons(params Coupon[] coupons)
    {
        var mockSet = coupons.ToList().BuildMockDbSet();
        _context.Coupons.Returns(mockSet);
    }

    private static CreateCouponCommand ValidCommand() => new(
        "SAVE10", "10% off", "Percentage", 10m, null, null,
        DateTime.UtcNow, DateTime.UtcNow.AddDays(30));

    [Fact]
    public async Task Handle_ValidCommand_ReturnsCouponResponse()
    {
        SetupCoupons();

        var result = await _handler.Handle(ValidCommand(), CancellationToken.None);

        result.Code.Should().Be("SAVE10");
        result.IsActive.Should().BeTrue();
        result.CurrentUsageCount.Should().Be(0);
    }

    [Fact]
    public async Task Handle_DuplicateCode_ThrowsBusinessRuleException()
    {
        SetupCoupons(new Coupon { Code = "SAVE10" });

        var act = () => _handler.Handle(ValidCommand(), CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*already exists*");
    }
}
