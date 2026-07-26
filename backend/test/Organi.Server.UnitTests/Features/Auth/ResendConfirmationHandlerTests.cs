using FluentAssertions;
using Microsoft.Extensions.Logging;
using MockQueryable.NSubstitute;
using NSubstitute;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Auth.Commands.ResendConfirmation;
using Organi.Server.Domain.Entities;
using Xunit;

namespace Organi.Server.UnitTests.Features.Auth;

public sealed class ResendConfirmationHandlerTests
{
    private const string Email = "jane@example.com";

    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly ITokenService _tokenService = Substitute.For<ITokenService>();
    private readonly IEmailService _emailService = Substitute.For<IEmailService>();
    private readonly ILogger<ResendConfirmationHandler> _logger = Substitute.For<ILogger<ResendConfirmationHandler>>();
    private readonly ResendConfirmationHandler _handler;

    public ResendConfirmationHandlerTests()
    {
        _tokenService.HashToken(Arg.Any<string>()).Returns(call => $"hashed-{call.Arg<string>()}");
        _handler = new ResendConfirmationHandler(_context, _tokenService, _emailService, _logger);
    }

    private void SetupUsers(params User[] users)
    {
        // Build the mock DbSet first — NSubstitute rejects configuring a substitute inside Returns().
        var mockUsers = users.ToList().BuildMockDbSet();

        _context.Users.Returns(mockUsers);
    }

    private static User CreateUser(bool emailConfirmed) => new()
    {
        Email = Email,
        PasswordHash = "hash",
        FirstName = "Jane",
        LastName = "Doe",
        EmailConfirmed = emailConfirmed
    };

    [Fact]
    public async Task Handle_UnconfirmedUser_IssuesNewTokenAndSends()
    {
        var user = CreateUser(emailConfirmed: false);

        SetupUsers(user);

        await _handler.Handle(new ResendConfirmationCommand(Email), CancellationToken.None);

        user.EmailConfirmationTokenHash.Should().NotBeNullOrEmpty();
        user.EmailConfirmationTokenExpiresAt.Should().BeAfter(DateTime.UtcNow);

        await _context.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        await _emailService.Received(1).SendEmailConfirmationAsync(
            Email, Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_AlreadyConfirmedUser_SendsNothing()
    {
        SetupUsers(CreateUser(emailConfirmed: true));

        await _handler.Handle(new ResendConfirmationCommand(Email), CancellationToken.None);

        await _context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        await _emailService.DidNotReceive().SendEmailConfirmationAsync(
            Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UnknownEmail_SendsNothingButDoesNotThrow()
    {
        SetupUsers();

        await _handler.Handle(new ResendConfirmationCommand("nobody@example.com"), CancellationToken.None);

        await _emailService.DidNotReceive().SendEmailConfirmationAsync(
            Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }
}
