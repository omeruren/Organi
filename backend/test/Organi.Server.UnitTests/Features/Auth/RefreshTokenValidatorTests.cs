using FluentAssertions;
using Organi.Server.Application.Features.Auth.Commands.Refresh;
using Xunit;

namespace Organi.Server.UnitTests.Features.Auth;

public sealed class RefreshTokenValidatorTests
{
    private readonly RefreshTokenValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_ReturnsSuccess()
    {
        var result = _validator.Validate(new RefreshTokenCommand("some-token-value"));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_EmptyToken_ReturnsError()
    {
        var result = _validator.Validate(new RefreshTokenCommand(""));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "RefreshToken");
    }
}
