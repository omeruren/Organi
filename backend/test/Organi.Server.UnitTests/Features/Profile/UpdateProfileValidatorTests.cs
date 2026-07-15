using FluentAssertions;
using Organi.Server.Application.Features.Profile.Commands.UpdateProfile;
using Xunit;

namespace Organi.Server.UnitTests.Features.Profile;

public sealed class UpdateProfileValidatorTests
{
    private readonly UpdateProfileValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_ReturnsSuccess()
    {
        var result = _validator.Validate(new UpdateProfileCommand("Ada", "Lovelace", "555-1234", null, null));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_EmptyFirstName_ReturnsError()
    {
        var result = _validator.Validate(new UpdateProfileCommand("", "Lovelace", null, null, null));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "FirstName");
    }
}
