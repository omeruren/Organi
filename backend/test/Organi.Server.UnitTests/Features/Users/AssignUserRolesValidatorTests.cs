using FluentAssertions;
using Organi.Server.Application.Features.Users.Commands.AssignUserRoles;
using Xunit;

namespace Organi.Server.UnitTests.Features.Users;

public sealed class AssignUserRolesValidatorTests
{
    private readonly AssignUserRolesValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_ReturnsSuccess()
    {
        var result = _validator.Validate(new AssignUserRolesCommand(["Customer"]));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_EmptyRoleList_ReturnsError()
    {
        var result = _validator.Validate(new AssignUserRolesCommand([]));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "RoleNames");
    }
}
