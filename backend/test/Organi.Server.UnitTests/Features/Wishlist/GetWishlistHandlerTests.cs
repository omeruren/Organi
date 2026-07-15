using FluentAssertions;
using MockQueryable.NSubstitute;
using NSubstitute;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Wishlist.Queries.GetWishlist;
using Organi.Server.Domain.Entities;
using Xunit;

namespace Organi.Server.UnitTests.Features.Wishlist;

public sealed class GetWishlistHandlerTests
{
    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
    private readonly GetWishlistHandler _handler;

    public GetWishlistHandlerTests()
    {
        _handler = new GetWishlistHandler(_context, _currentUser);
    }

    private void SetupWishlistItems(params WishlistItem[] items)
    {
        var mockSet = items.ToList().BuildMockDbSet();
        _context.WishlistItems.Returns(mockSet);
    }

    [Fact]
    public async Task Handle_NoItems_ReturnsEmptyListWithoutCreatingRows()
    {
        SetupWishlistItems();
        _currentUser.UserId.Returns(Guid.NewGuid());

        var result = await _handler.Handle(new GetWishlistQuery(), CancellationToken.None);

        result.Should().BeEmpty();
        _context.WishlistItems.DidNotReceive().Add(Arg.Any<WishlistItem>());
    }

    [Fact]
    public async Task Handle_MultipleItems_OrdersByMostRecentlyAddedFirst()
    {
        var userId = Guid.NewGuid();
        var older = new WishlistItem
        {
            UserId = userId,
            Product = new Product { Name = "Honey", Slug = "honey", SKU = "HNY-001" },
            CreatedAt = DateTime.UtcNow.AddDays(-5)
        };
        var newer = new WishlistItem
        {
            UserId = userId,
            Product = new Product { Name = "Apples", Slug = "apples", SKU = "APL-001" },
            CreatedAt = DateTime.UtcNow
        };
        SetupWishlistItems(older, newer);
        _currentUser.UserId.Returns(userId);

        var result = await _handler.Handle(new GetWishlistQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
        result[0].ProductName.Should().Be("Apples");
        result[1].ProductName.Should().Be("Honey");
    }
}
