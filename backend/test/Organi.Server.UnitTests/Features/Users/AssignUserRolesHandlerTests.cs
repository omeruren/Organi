using FluentAssertions;
using Microsoft.Extensions.Logging;
using MockQueryable.NSubstitute;
using NSubstitute;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Users.Commands.AssignUserRoles;
using Organi.Server.Domain.Entities;
using Organi.Server.Domain.Enums;
using Organi.Server.Domain.Exceptions;
using Xunit;

namespace Organi.Server.UnitTests.Features.Users;

public sealed class AssignUserRolesHandlerTests
{
    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
    private readonly IAuditService _auditService = Substitute.For<IAuditService>();
    private readonly ILogger<AssignUserRolesHandler> _logger = Substitute.For<ILogger<AssignUserRolesHandler>>();
    private readonly AssignUserRolesHandler _handler;

    public AssignUserRolesHandlerTests()
    {
        _handler = new AssignUserRolesHandler(_context, _currentUser, _auditService, _logger);
    }

    private void SetupUsers(params User[] users)
    {
        var mockSet = users.ToList().BuildMockDbSet();
        _context.Users.Returns(mockSet);
    }

    private void SetupRoles(params Role[] roles)
    {
        var mockSet = roles.ToList().BuildMockDbSet();
        _context.Roles.Returns(mockSet);
    }

    private static readonly Role CustomerRole = new() { Name = "Customer" };
    private static readonly Role VendorRole = new() { Name = "Vendor" };
    private static readonly Role AdminRole = new() { Name = "Admin" };

    [Fact]
    public async Task Handle_ValidRoles_ReplacesRoleSetAndLogsAssignedAndRemoved()
    {
        var user = new User { Email = "a@organi.test", FirstName = "Ada", LastName = "Lovelace", Roles = [CustomerRole] };
        SetupUsers(user);
        SetupRoles(CustomerRole, VendorRole, AdminRole);
        _currentUser.UserId.Returns(Guid.NewGuid());

        var result = await _handler.Handle(
            new AssignUserRolesCommand(["Vendor"]) { Id = user.Id }, CancellationToken.None);

        result.Roles.Should().ContainSingle().Which.Should().Be("Vendor");
        _auditService.Received(1).Log("User", user.Id.ToString(), AuditAction.RoleAssigned, Arg.Any<object?>(), Arg.Any<object>());
        _auditService.Received(1).Log("User", user.Id.ToString(), AuditAction.RoleRemoved, Arg.Any<object>(), Arg.Any<object?>());
    }

    [Fact]
    public async Task Handle_UnknownRoleName_ThrowsBusinessRuleException()
    {
        var user = new User { Email = "a@organi.test", FirstName = "Ada", LastName = "Lovelace", Roles = [] };
        SetupUsers(user);
        SetupRoles(CustomerRole, VendorRole, AdminRole);
        _currentUser.UserId.Returns(Guid.NewGuid());

        var act = () => _handler.Handle(
            new AssignUserRolesCommand(["NotARole"]) { Id = user.Id }, CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*do not exist*");
    }

    [Fact]
    public async Task Handle_SelfRemovingOwnAdminRole_ThrowsBusinessRuleException()
    {
        var user = new User { Email = "admin@organi.test", FirstName = "Ada", LastName = "Admin", Roles = [AdminRole] };
        SetupUsers(user);
        SetupRoles(CustomerRole, VendorRole, AdminRole);
        _currentUser.UserId.Returns(user.Id);

        var act = () => _handler.Handle(
            new AssignUserRolesCommand(["Customer"]) { Id = user.Id }, CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*own Admin role*");
    }

    [Fact]
    public async Task Handle_UserNotFound_ThrowsNotFoundException()
    {
        SetupUsers();
        SetupRoles(CustomerRole);
        _currentUser.UserId.Returns(Guid.NewGuid());

        var act = () => _handler.Handle(
            new AssignUserRolesCommand(["Customer"]) { Id = Guid.NewGuid() }, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
