using FluentAssertions;
using Organi.Server.Application.Features.Auth.Commands.ChangePassword;
using Xunit;

namespace Organi.Server.UnitTests.Features.Auth;

public sealed class ChangePasswordValidatorTests
{
    private readonly ChangePasswordValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_ReturnsSuccess()
    {
        var result = _validator.Validate(new ChangePasswordCommand("OldPass1!", "NewPass2@"));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_SameAsCurrentPassword_ReturnsError()
    {
        var result = _validator.Validate(new ChangePasswordCommand("SamePass1!", "SamePass1!"));

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_WeakNewPassword_ReturnsError()
    {
        var result = _validator.Validate(new ChangePasswordCommand("OldPass1!", "weak"));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "NewPassword");
    }
}
