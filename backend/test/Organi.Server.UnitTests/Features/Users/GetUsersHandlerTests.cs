using FluentAssertions;
using MockQueryable.NSubstitute;
using NSubstitute;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Users.Queries.GetUsers;
using Organi.Server.Domain.Entities;
using Xunit;

namespace Organi.Server.UnitTests.Features.Users;

public sealed class GetUsersHandlerTests
{
    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly GetUsersHandler _handler;

    public GetUsersHandlerTests()
    {
        _handler = new GetUsersHandler(_context);
    }

    private void SetupUsers(params User[] users)
    {
        var mockSet = users.ToList().BuildMockDbSet();
        _context.Users.Returns(mockSet);
    }

    [Fact]
    public async Task Handle_IsActiveFilter_ReturnsOnlyMatching()
    {
        var active = new User { Email = "active@organi.test", FirstName = "Ada", LastName = "Lovelace", IsActive = true };
        var inactive = new User { Email = "inactive@organi.test", FirstName = "Bob", LastName = "Smith", IsActive = false };
        SetupUsers(active, inactive);

        var result = await _handler.Handle(new GetUsersQuery(IsActive: true), CancellationToken.None);

        result.Items.Should().ContainSingle();
        result.Items[0].Email.Should().Be("active@organi.test");
    }

    [Fact]
    public async Task Handle_SearchFiltersByEmailOrName()
    {
        var match = new User { Email = "findme@organi.test", FirstName = "Ada", LastName = "Lovelace" };
        var noMatch = new User { Email = "other@organi.test", FirstName = "Bob", LastName = "Smith" };
        SetupUsers(match, noMatch);

        var result = await _handler.Handle(new GetUsersQuery(Search: "findme"), CancellationToken.None);

        result.Items.Should().ContainSingle();
        result.Items[0].Email.Should().Be("findme@organi.test");
    }
}
