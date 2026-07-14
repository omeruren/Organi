using FluentAssertions;
using Organi.Server.Application.Features.Cart.Commands.AddCartItem;
using Xunit;

namespace Organi.Server.UnitTests.Features.Cart;

public sealed class AddCartItemValidatorTests
{
    private readonly AddCartItemValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_ReturnsSuccess()
    {
        var result = _validator.Validate(new AddCartItemCommand(Guid.NewGuid(), 2));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_EmptyProductId_ReturnsError()
    {
        var result = _validator.Validate(new AddCartItemCommand(Guid.Empty, 2));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ProductId");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_InvalidQuantity_ReturnsError(int quantity)
    {
        var result = _validator.Validate(new AddCartItemCommand(Guid.NewGuid(), quantity));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Quantity");
    }
}
