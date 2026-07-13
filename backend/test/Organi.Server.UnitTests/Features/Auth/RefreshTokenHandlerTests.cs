using FluentAssertions;
using Microsoft.Extensions.Logging;
using MockQueryable.NSubstitute;
using NSubstitute;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Auth.Commands.Refresh;
using Organi.Server.Domain.Entities;
using Xunit;

namespace Organi.Server.UnitTests.Features.Auth;

public sealed class RefreshTokenHandlerTests
{
    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly ITokenService _tokenService = Substitute.For<ITokenService>();
    private readonly ILogger<RefreshTokenHandler> _logger = Substitute.For<ILogger<RefreshTokenHandler>>();
    private readonly RefreshTokenHandler _handler;

    public RefreshTokenHandlerTests()
    {
        _handler = new RefreshTokenHandler(_context, _tokenService, _logger);
    }

    private static User CreateUser() => new()
    {
        Email = "jane@example.com",
        FirstName = "Jane",
        LastName = "Doe",
        Roles = [new Role { Name = "Customer", Permissions = [] }]
    };

    private void SetupRefreshTokens(params RefreshToken[] tokens)
    {
        var mockTokens = tokens.ToList().BuildMockDbSet();
        _context.RefreshTokens.Returns(mockTokens);
    }

    [Fact]
    public async Task Handle_ValidToken_RotatesAndReturnsNewPair()
    {
        var user = CreateUser();
        var existingToken = new RefreshToken
        {
            TokenHash = "hashed-old-token",
            ExpiresAt = DateTime.UtcNow.AddDays(1),
            IsRevoked = false,
            UserId = user.Id,
            User = user
        };

        SetupRefreshTokens(existingToken);

        var accessToken = new AccessToken("access-token", DateTime.UtcNow.AddMinutes(15));
        var newRefreshToken = new GeneratedRefreshToken("new-token", DateTime.UtcNow.AddDays(7));

        _tokenService.HashToken("old-token").Returns("hashed-old-token");
        _tokenService.HashToken("new-token").Returns("hashed-new-token");
        _tokenService.GenerateAccessToken(Arg.Any<User>(), Arg.Any<IReadOnlyList<string>>(), Arg.Any<IReadOnlyList<string>>(), Arg.Any<Guid?>())
            .Returns(accessToken);
        _tokenService.GenerateRefreshToken().Returns(newRefreshToken);

        var result = await _handler.Handle(new RefreshTokenCommand("old-token"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.RefreshToken.Should().Be("new-token");
        existingToken.IsRevoked.Should().BeTrue();
        existingToken.ReplacedByTokenHash.Should().Be("hashed-new-token");
    }

    [Fact]
    public async Task Handle_UnknownToken_ReturnsInvalidTokenError()
    {
        SetupRefreshTokens();
        _tokenService.HashToken(Arg.Any<string>()).Returns("some-hash");

        var result = await _handler.Handle(new RefreshTokenCommand("unknown-token"), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("InvalidToken");
    }

    [Fact]
    public async Task Handle_RevokedToken_DetectsReuseAndRevokesAllSessions()
    {
        var user = CreateUser();
        var revokedToken = new RefreshToken
        {
            TokenHash = "hashed-stolen-token",
            ExpiresAt = DateTime.UtcNow.AddDays(1),
            IsRevoked = true,
            UserId = user.Id,
            User = user
        };
        var otherActiveToken = new RefreshToken
        {
            TokenHash = "hashed-other-token",
            ExpiresAt = DateTime.UtcNow.AddDays(1),
            IsRevoked = false,
            UserId = user.Id,
            User = user
        };

        SetupRefreshTokens(revokedToken, otherActiveToken);
        _tokenService.HashToken("stolen-token").Returns("hashed-stolen-token");

        var result = await _handler.Handle(new RefreshTokenCommand("stolen-token"), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("TokenReuseDetected");
        otherActiveToken.IsRevoked.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ExpiredToken_ReturnsTokenExpiredError()
    {
        var user = CreateUser();
        var expiredToken = new RefreshToken
        {
            TokenHash = "hashed-expired-token",
            ExpiresAt = DateTime.UtcNow.AddDays(-1),
            IsRevoked = false,
            UserId = user.Id,
            User = user
        };

        SetupRefreshTokens(expiredToken);
        _tokenService.HashToken("expired-token").Returns("hashed-expired-token");

        var result = await _handler.Handle(new RefreshTokenCommand("expired-token"), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("TokenExpired");
    }
}
