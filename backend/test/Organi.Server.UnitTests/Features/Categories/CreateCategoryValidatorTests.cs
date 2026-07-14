using FluentAssertions;
using Organi.Server.Application.Features.Categories.Commands.CreateCategory;
using Xunit;

namespace Organi.Server.UnitTests.Features.Categories;

public sealed class CreateCategoryValidatorTests
{
    private readonly CreateCategoryValidator _validator = new();

    private static CreateCategoryCommand ValidCommand() => new(
        "Vegetables", "Fresh vegetables.", null, 0, null);

    [Fact]
    public void Validate_ValidCommand_ReturnsSuccess()
    {
        var result = _validator.Validate(ValidCommand());

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_EmptyName_ReturnsError()
    {
        var result = _validator.Validate(ValidCommand() with { Name = "" });

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public void Validate_NegativeDisplayOrder_ReturnsError()
    {
        var result = _validator.Validate(ValidCommand() with { DisplayOrder = -1 });

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "DisplayOrder");
    }
}
