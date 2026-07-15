using FluentAssertions;
using Microsoft.Extensions.Logging;
using MockQueryable.NSubstitute;
using NSubstitute;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Wishlist.Commands.AddWishlistItem;
using Organi.Server.Domain.Entities;
using Organi.Server.Domain.Exceptions;
using Xunit;

namespace Organi.Server.UnitTests.Features.Wishlist;

public sealed class AddWishlistItemHandlerTests
{
    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
    private readonly ILogger<AddWishlistItemHandler> _logger = Substitute.For<ILogger<AddWishlistItemHandler>>();
    private readonly AddWishlistItemHandler _handler;

    public AddWishlistItemHandlerTests()
    {
        _handler = new AddWishlistItemHandler(_context, _currentUser, _logger);
    }

    private void SetupProducts(params Product[] products)
    {
        var mockSet = products.ToList().BuildMockDbSet();
        _context.Products.Returns(mockSet);
    }

    private void SetupWishlistItems(params WishlistItem[] items)
    {
        var mockSet = items.ToList().BuildMockDbSet();
        _context.WishlistItems.Returns(mockSet);
    }

    [Fact]
    public async Task Handle_ProductNotWishlisted_AddsNewItem()
    {
        var userId = Guid.NewGuid();
        var product = new Product { Name = "Honey", Slug = "honey", SKU = "HNY-001", Price = 5m };
        SetupProducts(product);
        SetupWishlistItems();
        _currentUser.UserId.Returns(userId);

        var result = await _handler.Handle(new AddWishlistItemCommand(product.Id), CancellationToken.None);

        result.ProductId.Should().Be(product.Id);
    }

    [Fact]
    public async Task Handle_ProductAlreadyWishlisted_ReturnsExistingWithoutError()
    {
        var userId = Guid.NewGuid();
        var product = new Product { Name = "Honey", Slug = "honey", SKU = "HNY-001", Price = 5m };
        var existing = new WishlistItem { UserId = userId, ProductId = product.Id, Product = product };
        SetupProducts(product);
        SetupWishlistItems(existing);
        _currentUser.UserId.Returns(userId);

        var result = await _handler.Handle(new AddWishlistItemCommand(product.Id), CancellationToken.None);

        result.Id.Should().Be(existing.Id);
    }

    [Fact]
    public async Task Handle_ProductNotFound_ThrowsNotFoundException()
    {
        SetupProducts();
        SetupWishlistItems();
        _currentUser.UserId.Returns(Guid.NewGuid());

        var act = () => _handler.Handle(new AddWishlistItemCommand(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
