using FluentAssertions;
using Microsoft.Extensions.Logging;
using MockQueryable.NSubstitute;
using NSubstitute;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Cart.Commands.UpdateCartItem;
using Organi.Server.Domain.Exceptions;
using Xunit;
using CartEntity = Organi.Server.Domain.Entities.Cart;
using CartItemEntity = Organi.Server.Domain.Entities.CartItem;
using ProductEntity = Organi.Server.Domain.Entities.Product;

namespace Organi.Server.UnitTests.Features.Cart;

public sealed class UpdateCartItemHandlerTests
{
    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
    private readonly ILogger<UpdateCartItemHandler> _logger = Substitute.For<ILogger<UpdateCartItemHandler>>();
    private readonly UpdateCartItemHandler _handler;

    public UpdateCartItemHandlerTests()
    {
        _handler = new UpdateCartItemHandler(_context, _currentUser, _logger);
    }

    private void SetupCarts(params CartEntity[] carts)
    {
        var mockSet = carts.ToList().BuildMockDbSet();
        _context.Carts.Returns(mockSet);
    }

    [Fact]
    public async Task Handle_NoCart_ThrowsNotFoundException()
    {
        _currentUser.UserId.Returns(Guid.NewGuid());
        SetupCarts();

        var act = () => _handler.Handle(new UpdateCartItemCommand(2) { Id = Guid.NewGuid() }, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_ItemNotInCart_ThrowsNotFoundException()
    {
        var userId = Guid.NewGuid();
        _currentUser.UserId.Returns(userId);
        SetupCarts(new CartEntity { UserId = userId, CartItems = [] });

        var act = () => _handler.Handle(new UpdateCartItemCommand(2) { Id = Guid.NewGuid() }, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_QuantityExceedsStock_ThrowsBusinessRuleException()
    {
        var userId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var product = new ProductEntity { Name = "Honey", Slug = "honey", SKU = "HNY-001", StockQuantity = 3, ProductImages = [] };
        var item = new CartItemEntity { Id = itemId, Product = product, Quantity = 2, UnitPrice = 10m };

        _currentUser.UserId.Returns(userId);
        SetupCarts(new CartEntity { UserId = userId, CartItems = [item] });

        var act = () => _handler.Handle(new UpdateCartItemCommand(5) { Id = itemId }, CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*Insufficient stock*");
    }

    [Fact]
    public async Task Handle_ValidCommand_UpdatesQuantity()
    {
        var userId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var product = new ProductEntity { Name = "Honey", Slug = "honey", SKU = "HNY-001", StockQuantity = 10, ProductImages = [] };
        var item = new CartItemEntity { Id = itemId, Product = product, Quantity = 2, UnitPrice = 10m };

        _currentUser.UserId.Returns(userId);
        SetupCarts(new CartEntity { UserId = userId, CartItems = [item] });

        var result = await _handler.Handle(new UpdateCartItemCommand(5) { Id = itemId }, CancellationToken.None);

        result.Items.Should().ContainSingle(i => i.Id == itemId && i.Quantity == 5);
    }
}
