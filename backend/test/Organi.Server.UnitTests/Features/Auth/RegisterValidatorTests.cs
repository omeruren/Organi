using FluentAssertions;
using Organi.Server.Application.Features.Auth.Commands.Register;
using Xunit;

namespace Organi.Server.UnitTests.Features.Auth;

public sealed class RegisterValidatorTests
{
    private readonly RegisterValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_ReturnsSuccess()
    {
        var command = new RegisterCommand("jane@example.com", "Str0ng!Pass", "Jane", "Doe", null);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_EmptyEmail_ReturnsError()
    {
        var command = new RegisterCommand("", "Str0ng!Pass", "Jane", "Doe", null);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Theory]
    [InlineData("short1!")]
    [InlineData("alllowercase1!")]
    [InlineData("ALLUPPERCASE1!")]
    [InlineData("NoDigitsHere!")]
    [InlineData("NoSpecialChar1")]
    public void Validate_WeakPassword_ReturnsError(string password)
    {
        var command = new RegisterCommand("jane@example.com", password, "Jane", "Doe", null);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }

    [Fact]
    public void Validate_EmptyFirstName_ReturnsError()
    {
        var command = new RegisterCommand("jane@example.com", "Str0ng!Pass", "", "Doe", null);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "FirstName");
    }
}
