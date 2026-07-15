using FluentAssertions;
using Microsoft.Extensions.Logging;
using MockQueryable.NSubstitute;
using NSubstitute;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Users.Commands.ActivateUser;
using Organi.Server.Domain.Entities;
using Organi.Server.Domain.Exceptions;
using Xunit;

namespace Organi.Server.UnitTests.Features.Users;

public sealed class ActivateUserHandlerTests
{
    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly IAuditService _auditService = Substitute.For<IAuditService>();
    private readonly ILogger<ActivateUserHandler> _logger = Substitute.For<ILogger<ActivateUserHandler>>();
    private readonly ActivateUserHandler _handler;

    public ActivateUserHandlerTests()
    {
        _handler = new ActivateUserHandler(_context, _auditService, _logger);
    }

    private void SetupUsers(params User[] users)
    {
        var mockSet = users.ToList().BuildMockDbSet();
        _context.Users.Returns(mockSet);
    }

    [Fact]
    public async Task Handle_InactiveUser_Activates()
    {
        var user = new User { Email = "a@organi.test", FirstName = "Ada", LastName = "Lovelace", IsActive = false };
        SetupUsers(user);

        var result = await _handler.Handle(new ActivateUserCommand(user.Id), CancellationToken.None);

        result.IsActive.Should().BeTrue();
        _auditService.Received(1).Log("User", user.Id.ToString(), Organi.Server.Domain.Enums.AuditAction.Update, Arg.Any<object?>(), Arg.Any<object>());
    }

    [Fact]
    public async Task Handle_AlreadyActive_ThrowsBusinessRuleException()
    {
        var user = new User { Email = "a@organi.test", FirstName = "Ada", LastName = "Lovelace", IsActive = true };
        SetupUsers(user);

        var act = () => _handler.Handle(new ActivateUserCommand(user.Id), CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleException>();
    }

    [Fact]
    public async Task Handle_UserNotFound_ThrowsNotFoundException()
    {
        SetupUsers();

        var act = () => _handler.Handle(new ActivateUserCommand(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
