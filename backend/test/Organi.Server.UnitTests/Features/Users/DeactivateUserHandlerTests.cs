using FluentAssertions;
using Microsoft.Extensions.Logging;
using MockQueryable.NSubstitute;
using NSubstitute;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Users.Commands.DeactivateUser;
using Organi.Server.Domain.Entities;
using Organi.Server.Domain.Exceptions;
using Xunit;

namespace Organi.Server.UnitTests.Features.Users;

public sealed class DeactivateUserHandlerTests
{
    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
    private readonly IAuditService _auditService = Substitute.For<IAuditService>();
    private readonly ILogger<DeactivateUserHandler> _logger = Substitute.For<ILogger<DeactivateUserHandler>>();
    private readonly DeactivateUserHandler _handler;

    public DeactivateUserHandlerTests()
    {
        _handler = new DeactivateUserHandler(_context, _currentUser, _auditService, _logger);
    }

    private void SetupUsers(params User[] users)
    {
        var mockSet = users.ToList().BuildMockDbSet();
        _context.Users.Returns(mockSet);
    }

    [Fact]
    public async Task Handle_ActiveUser_Deactivates()
    {
        var user = new User { Email = "a@organi.test", FirstName = "Ada", LastName = "Lovelace", IsActive = true };
        SetupUsers(user);
        _currentUser.UserId.Returns(Guid.NewGuid());

        var result = await _handler.Handle(new DeactivateUserCommand(user.Id), CancellationToken.None);

        result.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_SelfDeactivation_ThrowsBusinessRuleException()
    {
        var user = new User { Email = "a@organi.test", FirstName = "Ada", LastName = "Lovelace", IsActive = true };
        SetupUsers(user);
        _currentUser.UserId.Returns(user.Id);

        var act = () => _handler.Handle(new DeactivateUserCommand(user.Id), CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*own account*");
    }

    [Fact]
    public async Task Handle_AlreadyInactive_ThrowsBusinessRuleException()
    {
        var user = new User { Email = "a@organi.test", FirstName = "Ada", LastName = "Lovelace", IsActive = false };
        SetupUsers(user);
        _currentUser.UserId.Returns(Guid.NewGuid());

        var act = () => _handler.Handle(new DeactivateUserCommand(user.Id), CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*already inactive*");
    }

    [Fact]
    public async Task Handle_UserNotFound_ThrowsNotFoundException()
    {
        SetupUsers();
        _currentUser.UserId.Returns(Guid.NewGuid());

        var act = () => _handler.Handle(new DeactivateUserCommand(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
