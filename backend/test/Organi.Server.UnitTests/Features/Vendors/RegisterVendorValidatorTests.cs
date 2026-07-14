using FluentAssertions;
using Organi.Server.Application.Features.Vendors.Commands.RegisterVendor;
using Xunit;

namespace Organi.Server.UnitTests.Features.Vendors;

public sealed class RegisterVendorValidatorTests
{
    private readonly RegisterVendorValidator _validator = new();

    private static RegisterVendorCommand ValidCommand() => new(
        "Green Valley Farm", "Fresh organic produce.", null, null, null, null, null);

    [Fact]
    public void Validate_ValidCommand_ReturnsSuccess()
    {
        var result = _validator.Validate(ValidCommand());

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_EmptyStoreName_ReturnsError()
    {
        var result = _validator.Validate(ValidCommand() with { StoreName = "" });

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "StoreName");
    }
}
