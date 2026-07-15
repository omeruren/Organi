using FluentAssertions;
using MockQueryable.NSubstitute;
using NSubstitute;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Profile.Queries.GetProfile;
using Organi.Server.Domain.Entities;
using Organi.Server.Domain.Exceptions;
using Xunit;

namespace Organi.Server.UnitTests.Features.Profile;

public sealed class GetProfileHandlerTests
{
    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
    private readonly GetProfileHandler _handler;

    public GetProfileHandlerTests()
    {
        _handler = new GetProfileHandler(_context, _currentUser);
    }

    private void SetupUsers(params User[] users)
    {
        var mockSet = users.ToList().BuildMockDbSet();
        _context.Users.Returns(mockSet);
    }

    [Fact]
    public async Task Handle_ExistingUser_ReturnsProfile()
    {
        var user = new User { Email = "ada@organi.test", FirstName = "Ada", LastName = "Lovelace" };
        SetupUsers(user);
        _currentUser.UserId.Returns(user.Id);

        var result = await _handler.Handle(new GetProfileQuery(), CancellationToken.None);

        result.Email.Should().Be("ada@organi.test");
    }

    [Fact]
    public async Task Handle_UserNotFound_ThrowsNotFoundException()
    {
        SetupUsers();
        _currentUser.UserId.Returns(Guid.NewGuid());

        var act = () => _handler.Handle(new GetProfileQuery(), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
