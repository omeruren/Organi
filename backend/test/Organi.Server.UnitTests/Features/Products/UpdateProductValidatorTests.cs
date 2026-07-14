using FluentAssertions;
using Organi.Server.Application.Features.Products.Commands.UpdateProduct;
using Organi.Server.Domain.Enums;
using Xunit;

namespace Organi.Server.UnitTests.Features.Products;

public sealed class UpdateProductValidatorTests
{
    private readonly UpdateProductValidator _validator = new();

    private static UpdateProductCommand ValidCommand() => new(
        "Organic Honey",
        null,
        null,
        12.50m,
        null,
        "HNY-001",
        100,
        "jar",
        null,
        true,
        false,
        nameof(ProductStatus.Active),
        Guid.NewGuid(),
        null)
    { Id = Guid.NewGuid() };

    [Fact]
    public void Validate_ValidCommand_ReturnsSuccess()
    {
        var result = _validator.Validate(ValidCommand());

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_EmptyId_ReturnsError()
    {
        var command = ValidCommand() with { Id = Guid.Empty };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Id");
    }

    [Fact]
    public void Validate_InvalidStatus_ReturnsError()
    {
        var result = _validator.Validate(ValidCommand() with { Status = "NotAStatus" });

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Status");
    }
}
