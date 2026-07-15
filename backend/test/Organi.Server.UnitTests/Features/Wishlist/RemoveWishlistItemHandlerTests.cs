using FluentAssertions;
using Microsoft.Extensions.Logging;
using MockQueryable.NSubstitute;
using NSubstitute;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Wishlist.Commands.RemoveWishlistItem;
using Organi.Server.Domain.Entities;
using Organi.Server.Domain.Exceptions;
using Xunit;

namespace Organi.Server.UnitTests.Features.Wishlist;

public sealed class RemoveWishlistItemHandlerTests
{
    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
    private readonly ILogger<RemoveWishlistItemHandler> _logger = Substitute.For<ILogger<RemoveWishlistItemHandler>>();
    private readonly RemoveWishlistItemHandler _handler;

    public RemoveWishlistItemHandlerTests()
    {
        _handler = new RemoveWishlistItemHandler(_context, _currentUser, _logger);
    }

    private void SetupWishlistItems(params WishlistItem[] items)
    {
        var mockSet = items.ToList().BuildMockDbSet();
        _context.WishlistItems.Returns(mockSet);
    }

    [Fact]
    public async Task Handle_ExistingItem_RemovesIt()
    {
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var item = new WishlistItem { UserId = userId, ProductId = productId, Product = new Product { Name = "Honey", Slug = "honey", SKU = "HNY-001" } };
        SetupWishlistItems(item);
        _currentUser.UserId.Returns(userId);

        await _handler.Handle(new RemoveWishlistItemCommand(productId), CancellationToken.None);

        _context.WishlistItems.Received(1).Remove(item);
    }

    [Fact]
    public async Task Handle_ItemNotFound_ThrowsNotFoundException()
    {
        SetupWishlistItems();
        _currentUser.UserId.Returns(Guid.NewGuid());

        var act = () => _handler.Handle(new RemoveWishlistItemCommand(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
