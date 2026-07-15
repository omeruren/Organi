using FluentAssertions;
using MockQueryable.NSubstitute;
using NSubstitute;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Users.Queries.GetUserById;
using Organi.Server.Domain.Entities;
using Organi.Server.Domain.Exceptions;
using Xunit;

namespace Organi.Server.UnitTests.Features.Users;

public sealed class GetUserByIdHandlerTests
{
    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly GetUserByIdHandler _handler;

    public GetUserByIdHandlerTests()
    {
        _handler = new GetUserByIdHandler(_context);
    }

    private void SetupUsers(params User[] users)
    {
        var mockSet = users.ToList().BuildMockDbSet();
        _context.Users.Returns(mockSet);
    }

    [Fact]
    public async Task Handle_ExistingUser_ReturnsResponse()
    {
        var user = new User { Email = "a@organi.test", FirstName = "Ada", LastName = "Lovelace" };
        SetupUsers(user);

        var result = await _handler.Handle(new GetUserByIdQuery(user.Id), CancellationToken.None);

        result.Email.Should().Be("a@organi.test");
    }

    [Fact]
    public async Task Handle_UserNotFound_ThrowsNotFoundException()
    {
        SetupUsers();

        var act = () => _handler.Handle(new GetUserByIdQuery(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
