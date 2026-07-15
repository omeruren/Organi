using FluentAssertions;
using Microsoft.Extensions.Logging;
using MockQueryable.NSubstitute;
using NSubstitute;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Profile.Commands.UpdateProfile;
using Organi.Server.Domain.Entities;
using Xunit;

namespace Organi.Server.UnitTests.Features.Profile;

public sealed class UpdateProfileHandlerTests
{
    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
    private readonly ILogger<UpdateProfileHandler> _logger = Substitute.For<ILogger<UpdateProfileHandler>>();
    private readonly UpdateProfileHandler _handler;

    public UpdateProfileHandlerTests()
    {
        _handler = new UpdateProfileHandler(_context, _currentUser, _logger);
    }

    private void SetupUsers(params User[] users)
    {
        var mockSet = users.ToList().BuildMockDbSet();
        _context.Users.Returns(mockSet);
    }

    [Fact]
    public async Task Handle_ValidCommand_UpdatesProfileFields()
    {
        var user = new User { Email = "ada@organi.test", FirstName = "Ada", LastName = "Lovelace" };
        SetupUsers(user);
        _currentUser.UserId.Returns(user.Id);

        var result = await _handler.Handle(
            new UpdateProfileCommand("Ada", "King", "555-1234", null, null), CancellationToken.None);

        result.LastName.Should().Be("King");
        result.PhoneNumber.Should().Be("555-1234");
    }
}
