using FluentAssertions;
using Microsoft.Extensions.Logging;
using MockQueryable.NSubstitute;
using NSubstitute;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Auth;
using Organi.Server.Application.Features.Auth.Commands.ResetPassword;
using Organi.Server.Domain.Entities;
using Organi.Server.Domain.Exceptions;
using Xunit;

namespace Organi.Server.UnitTests.Features.Auth;

public sealed class ResetPasswordHandlerTests
{
    private const string Email = "jane@example.com";
    private const string Code = "123456";
    private const string CodeHash = "hashed-123456";
    private const string NewPassword = "Str0ng!NewPass";

    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly ITokenService _tokenService = Substitute.For<ITokenService>();
    private readonly IAuditService _auditService = Substitute.For<IAuditService>();
    private readonly ILogger<ResetPasswordHandler> _logger = Substitute.For<ILogger<ResetPasswordHandler>>();
    private readonly ResetPasswordHandler _handler;

    public ResetPasswordHandlerTests()
    {
        _tokenService.HashToken(Code).Returns(CodeHash);
        _passwordHasher.Hash(NewPassword).Returns("new-password-hash");
        _handler = new ResetPasswordHandler(_context, _passwordHasher, _tokenService, _auditService, _logger);
    }

    private static User CreateUser(string? codeHash = CodeHash, DateTime? expiresAt = null, int attempts = 0) => new()
    {
        Email = Email,
        PasswordHash = "old-password-hash",
        FirstName = "Jane",
        LastName = "Doe",
        PasswordResetCodeHash = codeHash,
        PasswordResetCodeExpiresAt = expiresAt ?? DateTime.UtcNow.AddMinutes(10),
        PasswordResetAttemptCount = attempts
    };

    private void Setup(User? user, params RefreshToken[] refreshTokens)
    {
        // Build the mock DbSets first — NSubstitute rejects configuring a substitute inside Returns().
        var users = user is null ? new List<User>() : [user];
        var mockUsers = users.BuildMockDbSet();
        var mockRefreshTokens = refreshTokens.ToList().BuildMockDbSet();

        _context.Users.Returns(mockUsers);
        _context.RefreshTokens.Returns(mockRefreshTokens);
    }

    private Task Act() => _handler.Handle(new ResetPasswordCommand(Email, Code, NewPassword), CancellationToken.None);

    [Fact]
    public async Task Handle_ValidCode_SetsNewPasswordAndClearsResetState()
    {
        var user = CreateUser();

        Setup(user);

        await Act();

        user.PasswordHash.Should().Be("new-password-hash");
        user.PasswordResetCodeHash.Should().BeNull("the code must be single-use");
        user.PasswordResetCodeExpiresAt.Should().BeNull();
        user.PasswordResetAttemptCount.Should().Be(0);
    }

    [Fact]
    public async Task Handle_ValidCode_RevokesAllActiveSessions()
    {
        var user = CreateUser();

        var active = new RefreshToken { TokenHash = "a", ExpiresAt = DateTime.UtcNow.AddDays(1), UserId = user.Id };
        var otherUsers = new RefreshToken { TokenHash = "b", ExpiresAt = DateTime.UtcNow.AddDays(1), UserId = Guid.NewGuid() };

        Setup(user, active, otherUsers);

        await Act();

        active.IsRevoked.Should().BeTrue();
        active.RevokedAt.Should().NotBeNull();
        otherUsers.IsRevoked.Should().BeFalse("another user's sessions must be untouched");
    }

    [Fact]
    public async Task Handle_ValidCode_MarksEmailConfirmedAndClearsLockout()
    {
        var user = CreateUser();

        user.FailedLoginCount = 4;
        user.LockoutEnd = DateTime.UtcNow.AddMinutes(20);

        Setup(user);

        await Act();

        // Receiving the emailed code proves inbox control.
        user.EmailConfirmed.Should().BeTrue();
        user.FailedLoginCount.Should().Be(0);
        user.LockoutEnd.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WrongCode_IncrementsAttemptCountAndKeepsOldPassword()
    {
        var user = CreateUser(codeHash: "hash-of-a-different-code");

        Setup(user);

        var act = () => Act();

        await act.Should().ThrowAsync<BusinessRuleException>();
        user.PasswordHash.Should().Be("old-password-hash");
        user.PasswordResetAttemptCount.Should().Be(1, "failed guesses must be persisted to cap brute force");
    }

    [Fact]
    public async Task Handle_ExpiredCode_ThrowsAndKeepsOldPassword()
    {
        var user = CreateUser(expiresAt: DateTime.UtcNow.AddMinutes(-1));

        Setup(user);

        var act = () => Act();

        await act.Should().ThrowAsync<BusinessRuleException>();
        user.PasswordHash.Should().Be("old-password-hash");
    }

    [Fact]
    public async Task Handle_TooManyAttempts_BurnsCodeAndThrows()
    {
        var user = CreateUser(attempts: AuthVerificationPolicy.MaxPasswordResetAttempts);

        Setup(user);

        var act = () => Act();

        await act.Should().ThrowAsync<BusinessRuleException>().WithMessage("*Too many*");
        user.PasswordResetCodeHash.Should().BeNull("the code is burned once the attempt cap is hit");
        user.PasswordHash.Should().Be("old-password-hash");
    }

    [Fact]
    public async Task Handle_NoResetRequested_Throws()
    {
        Setup(CreateUser(codeHash: null));

        var act = () => Act();

        await act.Should().ThrowAsync<BusinessRuleException>();
    }

    [Fact]
    public async Task Handle_UnknownEmail_ThrowsSameErrorAsWrongCode()
    {
        Setup(user: null);

        var act = () => Act();

        // Identical message to the wrong-code path, so the endpoint reveals nothing about
        // whether the address is registered.
        await act.Should().ThrowAsync<BusinessRuleException>().WithMessage("*invalid or has expired*");
    }
}
