using FluentAssertions;
using Organi.Server.Application.Features.Coupons.Commands.CreateCoupon;
using Xunit;

namespace Organi.Server.UnitTests.Features.Coupons;

public sealed class CreateCouponValidatorTests
{
    private readonly CreateCouponValidator _validator = new();

    private static CreateCouponCommand ValidCommand(string discountType = "Percentage", decimal discountValue = 10m) => new(
        "SAVE10", "10% off", discountType, discountValue, null, null,
        DateTime.UtcNow, DateTime.UtcNow.AddDays(30));

    [Fact]
    public void Validate_ValidCommand_ReturnsSuccess()
    {
        var result = _validator.Validate(ValidCommand());

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_EmptyCode_ReturnsError()
    {
        var result = _validator.Validate(ValidCommand() with { Code = "" });

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Code");
    }

    [Fact]
    public void Validate_InvalidDiscountType_ReturnsError()
    {
        var result = _validator.Validate(ValidCommand() with { DiscountType = "NotAType" });

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "DiscountType");
    }

    [Fact]
    public void Validate_PercentageOver100_ReturnsError()
    {
        var result = _validator.Validate(ValidCommand(discountType: "Percentage", discountValue: 150m));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "DiscountValue");
    }

    [Fact]
    public void Validate_FixedAmountOver100_ReturnsSuccess()
    {
        var result = _validator.Validate(ValidCommand(discountType: "FixedAmount", discountValue: 150m));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_EndDateBeforeStartDate_ReturnsError()
    {
        var result = _validator.Validate(ValidCommand() with { StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(-1) });

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "EndDate");
    }
}
