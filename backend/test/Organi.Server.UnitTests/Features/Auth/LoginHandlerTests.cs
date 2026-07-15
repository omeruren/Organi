using FluentAssertions;
using Microsoft.Extensions.Logging;
using MockQueryable.NSubstitute;
using NSubstitute;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Auth.Commands.Login;
using Organi.Server.Domain.Entities;
using Xunit;

namespace Organi.Server.UnitTests.Features.Auth;

public sealed class LoginHandlerTests
{
    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly ITokenService _tokenService = Substitute.For<ITokenService>();
    private readonly IAuditService _auditService = Substitute.For<IAuditService>();
    private readonly ILogger<LoginHandler> _logger = Substitute.For<ILogger<LoginHandler>>();
    private readonly LoginHandler _handler;

    public LoginHandlerTests()
    {
        _handler = new LoginHandler(_context, _passwordHasher, _tokenService, _auditService, _logger);
    }

    private static User CreateUser(bool isActive = true, DateTime? lockoutEnd = null, int failedLoginCount = 0) => new()
    {
        Email = "jane@example.com",
        PasswordHash = "hashed-password",
        FirstName = "Jane",
        LastName = "Doe",
        IsActive = isActive,
        LockoutEnd = lockoutEnd,
        FailedLoginCount = failedLoginCount,
        Roles = [new Role { Name = "Customer", Permissions = [] }]
    };

    private void SetupUsers(params User[] users)
    {
        var mockUsers = users.ToList().BuildMockDbSet();
        _context.Users.Returns(mockUsers);
    }

    private void SetupRefreshTokens(params RefreshToken[] tokens)
    {
        var mockTokens = tokens.ToList().BuildMockDbSet();
        _context.RefreshTokens.Returns(mockTokens);
    }

    private void StubTokenGeneration()
    {
        var accessToken = new AccessToken("access-token", DateTime.UtcNow.AddMinutes(15));
        var refreshToken = new GeneratedRefreshToken("refresh-token", DateTime.UtcNow.AddDays(7));

        _tokenService.GenerateAccessToken(Arg.Any<User>(), Arg.Any<IReadOnlyList<string>>(), Arg.Any<IReadOnlyList<string>>(), Arg.Any<Guid?>())
            .Returns(accessToken);
        _tokenService.GenerateRefreshToken().Returns(refreshToken);
    }

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsSuccessResult()
    {
        var user = CreateUser();
        SetupUsers(user);
        SetupRefreshTokens();
        _passwordHasher.Verify("correct-password", user.PasswordHash).Returns(true);
        StubTokenGeneration();

        var result = await _handler.Handle(new LoginCommand(user.Email, "correct-password"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.AccessToken.Should().Be("access-token");
    }

    [Fact]
    public async Task Handle_WrongPassword_ReturnsInvalidCredentialsAndIncrementsFailedCount()
    {
        var user = CreateUser();
        SetupUsers(user);
        _passwordHasher.Verify("wrong-password", user.PasswordHash).Returns(false);

        var result = await _handler.Handle(new LoginCommand(user.Email, "wrong-password"), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("InvalidCredentials");
        user.FailedLoginCount.Should().Be(1);
    }

    [Fact]
    public async Task Handle_FifthFailedAttempt_LocksAccount()
    {
        var user = CreateUser(failedLoginCount: 4);
        SetupUsers(user);
        _passwordHasher.Verify(Arg.Any<string>(), Arg.Any<string>()).Returns(false);

        var result = await _handler.Handle(new LoginCommand(user.Email, "wrong-password"), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        user.FailedLoginCount.Should().Be(5);
        user.LockoutEnd.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_LockedAccount_ReturnsAccountLockedError()
    {
        var user = CreateUser(lockoutEnd: DateTime.UtcNow.AddMinutes(10));
        SetupUsers(user);

        var result = await _handler.Handle(new LoginCommand(user.Email, "any-password"), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("AccountLocked");
    }

    [Fact]
    public async Task Handle_ExpiredLockout_AutoUnlocksAndAllowsLogin()
    {
        var user = CreateUser(lockoutEnd: DateTime.UtcNow.AddMinutes(-1), failedLoginCount: 5);
        SetupUsers(user);
        SetupRefreshTokens();
        _passwordHasher.Verify("correct-password", user.PasswordHash).Returns(true);
        StubTokenGeneration();

        var result = await _handler.Handle(new LoginCommand(user.Email, "correct-password"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        user.LockoutEnd.Should().BeNull();
        user.FailedLoginCount.Should().Be(0);
    }

    [Fact]
    public async Task Handle_DeactivatedAccount_ReturnsAccountDeactivatedError()
    {
        var user = CreateUser(isActive: false);
        SetupUsers(user);

        var result = await _handler.Handle(new LoginCommand(user.Email, "any-password"), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("AccountDeactivated");
    }

    [Fact]
    public async Task Handle_UnknownEmail_ReturnsInvalidCredentialsError()
    {
        SetupUsers();

        var result = await _handler.Handle(new LoginCommand("unknown@example.com", "any-password"), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("InvalidCredentials");
    }
}
