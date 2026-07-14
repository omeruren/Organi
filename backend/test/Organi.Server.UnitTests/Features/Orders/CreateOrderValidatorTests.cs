using FluentAssertions;
using Organi.Server.Application.Features.Orders.Commands.CreateOrder;
using Xunit;

namespace Organi.Server.UnitTests.Features.Orders;

public sealed class CreateOrderValidatorTests
{
    private readonly CreateOrderValidator _validator = new();

    private static CreateOrderCommand ValidCommand() => new(
        "Jane", "Doe", "123 Main St", "Springfield", "12345", "555-0100", "jane@example.com", null, null);

    [Fact]
    public void Validate_ValidCommand_ReturnsSuccess()
    {
        var result = _validator.Validate(ValidCommand());

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_EmptyShippingAddress_ReturnsError()
    {
        var result = _validator.Validate(ValidCommand() with { ShippingAddress = "" });

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ShippingAddress");
    }

    [Fact]
    public void Validate_InvalidShippingEmail_ReturnsError()
    {
        var result = _validator.Validate(ValidCommand() with { ShippingEmail = "not-an-email" });

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ShippingEmail");
    }
}
