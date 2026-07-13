using FluentAssertions;
using Microsoft.Extensions.Logging;
using MockQueryable.NSubstitute;
using NSubstitute;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Auth.Commands.Register;
using Organi.Server.Domain.Entities;
using Organi.Server.Domain.Exceptions;
using Xunit;

namespace Organi.Server.UnitTests.Features.Auth;

public sealed class RegisterHandlerTests
{
    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly ITokenService _tokenService = Substitute.For<ITokenService>();
    private readonly ILogger<RegisterHandler> _logger = Substitute.For<ILogger<RegisterHandler>>();
    private readonly RegisterHandler _handler;

    public RegisterHandlerTests()
    {
        _handler = new RegisterHandler(_context, _passwordHasher, _tokenService, _logger);
    }

    private static Role CreateCustomerRole() => new()
    {
        Name = "Customer",
        Permissions = [new Permission { Name = "Products.Read", Module = "Products" }]
    };

    [Fact]
    public async Task Handle_ValidCommand_ReturnsAuthResponse()
    {
        var command = new RegisterCommand("jane@example.com", "Str0ng!Pass", "Jane", "Doe", null);

        var mockUsers = new List<User>().BuildMockDbSet();
        var mockRoles = new List<Role> { CreateCustomerRole() }.BuildMockDbSet();
        var mockRefreshTokens = new List<RefreshToken>().BuildMockDbSet();
        _context.Users.Returns(mockUsers);
        _context.Roles.Returns(mockRoles);
        _context.RefreshTokens.Returns(mockRefreshTokens);

        var accessToken = new AccessToken("access-token", DateTime.UtcNow.AddMinutes(15));
        var refreshToken = new GeneratedRefreshToken("refresh-token", DateTime.UtcNow.AddDays(7));

        _passwordHasher.Hash(command.Password).Returns("hashed-password");
        _tokenService.GenerateAccessToken(Arg.Any<User>(), Arg.Any<IReadOnlyList<string>>(), Arg.Any<IReadOnlyList<string>>(), Arg.Any<Guid?>())
            .Returns(accessToken);
        _tokenService.GenerateRefreshToken().Returns(refreshToken);
        _tokenService.HashToken(Arg.Any<string>()).Returns("hashed-refresh-token");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.AccessToken.Should().Be("access-token");
        result.RefreshToken.Should().Be("refresh-token");
    }

    [Fact]
    public async Task Handle_DuplicateEmail_ThrowsBusinessRuleException()
    {
        var command = new RegisterCommand("jane@example.com", "Str0ng!Pass", "Jane", "Doe", null);

        var existingUser = new User { Email = "jane@example.com" };
        var mockUsers = new List<User> { existingUser }.BuildMockDbSet();
        _context.Users.Returns(mockUsers);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*already registered*");
    }
}
