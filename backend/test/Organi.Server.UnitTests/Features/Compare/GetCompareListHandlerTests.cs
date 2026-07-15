using FluentAssertions;
using MockQueryable.NSubstitute;
using NSubstitute;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Compare.Queries.GetCompareList;
using Organi.Server.Domain.Entities;
using Xunit;

namespace Organi.Server.UnitTests.Features.Compare;

public sealed class GetCompareListHandlerTests
{
    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
    private readonly GetCompareListHandler _handler;

    public GetCompareListHandlerTests()
    {
        _handler = new GetCompareListHandler(_context, _currentUser);
    }

    private void SetupCompareItems(params CompareItem[] items)
    {
        var mockSet = items.ToList().BuildMockDbSet();
        _context.CompareItems.Returns(mockSet);
    }

    [Fact]
    public async Task Handle_NoItems_ReturnsEmptyListWithoutCreatingRows()
    {
        SetupCompareItems();
        _currentUser.UserId.Returns(Guid.NewGuid());

        var result = await _handler.Handle(new GetCompareListQuery(), CancellationToken.None);

        result.Should().BeEmpty();
        _context.CompareItems.DidNotReceive().Add(Arg.Any<CompareItem>());
    }

    [Fact]
    public async Task Handle_MultipleItems_OrdersByMostRecentlyAddedFirst()
    {
        var userId = Guid.NewGuid();
        var older = new CompareItem
        {
            UserId = userId,
            Product = new Product { Name = "Honey", Slug = "honey", SKU = "HNY-001" },
            CreatedAt = DateTime.UtcNow.AddDays(-5)
        };
        var newer = new CompareItem
        {
            UserId = userId,
            Product = new Product { Name = "Apples", Slug = "apples", SKU = "APL-001" },
            CreatedAt = DateTime.UtcNow
        };
        SetupCompareItems(older, newer);
        _currentUser.UserId.Returns(userId);

        var result = await _handler.Handle(new GetCompareListQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
        result[0].ProductName.Should().Be("Apples");
        result[1].ProductName.Should().Be("Honey");
    }
}
