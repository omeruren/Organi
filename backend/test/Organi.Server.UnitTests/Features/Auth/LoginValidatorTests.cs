using FluentAssertions;
using Organi.Server.Application.Features.Auth.Commands.Login;
using Xunit;

namespace Organi.Server.UnitTests.Features.Auth;

public sealed class LoginValidatorTests
{
    private readonly LoginValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_ReturnsSuccess()
    {
        var result = _validator.Validate(new LoginCommand("jane@example.com", "anything"));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_InvalidEmail_ReturnsError()
    {
        var result = _validator.Validate(new LoginCommand("not-an-email", "anything"));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Fact]
    public void Validate_EmptyPassword_ReturnsError()
    {
        var result = _validator.Validate(new LoginCommand("jane@example.com", ""));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }
}
