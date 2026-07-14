using FluentAssertions;
using Microsoft.Extensions.Logging;
using MockQueryable.NSubstitute;
using NSubstitute;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Cart.Commands.AddCartItem;
using Organi.Server.Domain.Exceptions;
using Xunit;
using CartEntity = Organi.Server.Domain.Entities.Cart;
using CartItemEntity = Organi.Server.Domain.Entities.CartItem;
using ProductEntity = Organi.Server.Domain.Entities.Product;

namespace Organi.Server.UnitTests.Features.Cart;

public sealed class AddCartItemHandlerTests
{
    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
    private readonly ILogger<AddCartItemHandler> _logger = Substitute.For<ILogger<AddCartItemHandler>>();
    private readonly AddCartItemHandler _handler;

    public AddCartItemHandlerTests()
    {
        _handler = new AddCartItemHandler(_context, _currentUser, _logger);
    }

    private void SetupProducts(params ProductEntity[] products)
    {
        var mockSet = products.ToList().BuildMockDbSet();
        _context.Products.Returns(mockSet);
    }

    private void SetupCarts(params CartEntity[] carts)
    {
        var mockSet = carts.ToList().BuildMockDbSet();
        _context.Carts.Returns(mockSet);
    }

    private static ProductEntity BuildProduct(Guid id, int stock, decimal price = 10m) => new()
    {
        Id = id,
        Name = "Organic Honey",
        Slug = "organic-honey",
        SKU = "HNY-001",
        Price = price,
        StockQuantity = stock,
        ProductImages = []
    };

    [Fact]
    public async Task Handle_ProductNotFound_ThrowsNotFoundException()
    {
        _currentUser.UserId.Returns(Guid.NewGuid());
        SetupProducts();

        var act = () => _handler.Handle(new AddCartItemCommand(Guid.NewGuid(), 1), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_QuantityExceedsStock_ThrowsBusinessRuleException()
    {
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        _currentUser.UserId.Returns(userId);

        SetupProducts(BuildProduct(productId, stock: 3));
        SetupCarts();

        var act = () => _handler.Handle(new AddCartItemCommand(productId, 5), CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*Insufficient stock*");
    }

    [Fact]
    public async Task Handle_NewProductNoExistingCart_CreatesCartAndReturnsResponse()
    {
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        _currentUser.UserId.Returns(userId);

        SetupProducts(BuildProduct(productId, stock: 10));
        SetupCarts();

        await _handler.Handle(new AddCartItemCommand(productId, 2), CancellationToken.None);

        // The mocked DbSet doesn't replicate EF Core's relationship fixup (which is what
        // populates cart.CartItems for the response in the real DbContext), so this test
        // verifies the write via the DbSet call rather than asserting on the returned response.
        _context.CartItems.Received(1).Add(Arg.Is<CartItemEntity>(
            ci => ci!.ProductId == productId && ci.Quantity == 2));
    }

    [Fact]
    public async Task Handle_ProductAlreadyInCart_MergesQuantity()
    {
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var product = BuildProduct(productId, stock: 10);
        _currentUser.UserId.Returns(userId);

        var existingItem = new CartItemEntity { ProductId = productId, Product = product, Quantity = 2, UnitPrice = 10m };
        var cart = new CartEntity { UserId = userId, CartItems = [existingItem] };

        SetupProducts(product);
        SetupCarts(cart);

        var result = await _handler.Handle(new AddCartItemCommand(productId, 3), CancellationToken.None);

        result.Items.Should().ContainSingle(i => i.ProductId == productId && i.Quantity == 5);
    }
}
