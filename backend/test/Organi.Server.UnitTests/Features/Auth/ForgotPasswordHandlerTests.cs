using FluentAssertions;
using Microsoft.Extensions.Logging;
using MockQueryable.NSubstitute;
using NSubstitute;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Auth.Commands.ForgotPassword;
using Organi.Server.Domain.Entities;
using Xunit;

namespace Organi.Server.UnitTests.Features.Auth;

public sealed class ForgotPasswordHandlerTests
{
    private const string Email = "jane@example.com";

    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly ITokenService _tokenService = Substitute.For<ITokenService>();
    private readonly IEmailService _emailService = Substitute.For<IEmailService>();
    private readonly ILogger<ForgotPasswordHandler> _logger = Substitute.For<ILogger<ForgotPasswordHandler>>();
    private readonly ForgotPasswordHandler _handler;

    public ForgotPasswordHandlerTests()
    {
        _tokenService.HashToken(Arg.Any<string>()).Returns(call => $"hashed-{call.Arg<string>()}");
        _handler = new ForgotPasswordHandler(_context, _tokenService, _emailService, _logger);
    }

    private void SetupUsers(params User[] users)
    {
        // Build the mock DbSet first — NSubstitute rejects configuring a substitute inside Returns().
        var mockUsers = users.ToList().BuildMockDbSet();

        _context.Users.Returns(mockUsers);
    }

    private static User CreateUser(bool isActive = true) => new()
    {
        Email = Email,
        PasswordHash = "hash",
        FirstName = "Jane",
        LastName = "Doe",
        IsActive = isActive
    };

    [Fact]
    public async Task Handle_KnownActiveUser_StoresHashedCodeAndSendsEmail()
    {
        var user = CreateUser();

        SetupUsers(user);

        await _handler.Handle(new ForgotPasswordCommand(Email), CancellationToken.None);

        user.PasswordResetCodeHash.Should().NotBeNullOrEmpty();
        user.PasswordResetCodeExpiresAt.Should().BeAfter(DateTime.UtcNow);
        user.PasswordResetAttemptCount.Should().Be(0);

        await _context.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        await _emailService.Received(1).SendPasswordResetCodeAsync(
            Email, Arg.Any<string>(), Arg.Is<string>(code => code != null && code.Length == 6), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UnknownEmail_SendsNothingButDoesNotThrow()
    {
        SetupUsers();

        await _handler.Handle(new ForgotPasswordCommand("nobody@example.com"), CancellationToken.None);

        // Silent success is what stops this endpoint being an account-enumeration oracle.
        await _context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        await _emailService.DidNotReceive().SendPasswordResetCodeAsync(
            Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_DeactivatedUser_SendsNothing()
    {
        SetupUsers(CreateUser(isActive: false));

        await _handler.Handle(new ForgotPasswordCommand(Email), CancellationToken.None);

        await _emailService.DidNotReceive().SendPasswordResetCodeAsync(
            Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }
}
