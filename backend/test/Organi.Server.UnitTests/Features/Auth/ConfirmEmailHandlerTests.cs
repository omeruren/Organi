using FluentAssertions;
using Microsoft.Extensions.Logging;
using MockQueryable.NSubstitute;
using NSubstitute;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Auth.Commands.ConfirmEmail;
using Organi.Server.Domain.Entities;
using Organi.Server.Domain.Exceptions;
using Xunit;

namespace Organi.Server.UnitTests.Features.Auth;

public sealed class ConfirmEmailHandlerTests
{
    private const string RawToken = "raw-confirmation-token";
    private const string TokenHash = "hashed-confirmation-token";

    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly ITokenService _tokenService = Substitute.For<ITokenService>();
    private readonly ILogger<ConfirmEmailHandler> _logger = Substitute.For<ILogger<ConfirmEmailHandler>>();
    private readonly ConfirmEmailHandler _handler;

    public ConfirmEmailHandlerTests()
    {
        _tokenService.HashToken(RawToken).Returns(TokenHash);
        _handler = new ConfirmEmailHandler(_context, _tokenService, _logger);
    }

    private void SetupUsers(params User[] users)
    {
        // Build the mock DbSet first — NSubstitute rejects configuring a substitute inside Returns().
        var mockUsers = users.ToList().BuildMockDbSet();

        _context.Users.Returns(mockUsers);
    }

    private static User CreateUser(string? tokenHash, DateTime? expiresAt) => new()
    {
        Email = "jane@example.com",
        PasswordHash = "hash",
        FirstName = "Jane",
        LastName = "Doe",
        EmailConfirmationTokenHash = tokenHash,
        EmailConfirmationTokenExpiresAt = expiresAt
    };

    [Fact]
    public async Task Handle_ValidToken_ConfirmsEmailAndClearsToken()
    {
        var user = CreateUser(TokenHash, DateTime.UtcNow.AddHours(1));

        SetupUsers(user);

        await _handler.Handle(new ConfirmEmailCommand(RawToken), CancellationToken.None);

        user.EmailConfirmed.Should().BeTrue();
        user.EmailConfirmationTokenHash.Should().BeNull("the token must be single-use");
        user.EmailConfirmationTokenExpiresAt.Should().BeNull();
        await _context.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UnknownToken_ThrowsAndSavesNothing()
    {
        SetupUsers(CreateUser("some-other-hash", DateTime.UtcNow.AddHours(1)));

        var act = () => _handler.Handle(new ConfirmEmailCommand(RawToken), CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleException>();
        await _context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ExpiredToken_ThrowsAndLeavesEmailUnconfirmed()
    {
        var user = CreateUser(TokenHash, DateTime.UtcNow.AddMinutes(-1));

        SetupUsers(user);

        var act = () => _handler.Handle(new ConfirmEmailCommand(RawToken), CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleException>().WithMessage("*expired*");
        user.EmailConfirmed.Should().BeFalse();
        await _context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
