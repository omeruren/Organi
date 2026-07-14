using FluentAssertions;
using Microsoft.Extensions.Logging;
using MockQueryable.NSubstitute;
using NSubstitute;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Orders.Commands.CreateOrder;
using Organi.Server.Domain.Entities;
using Organi.Server.Domain.Exceptions;
using Xunit;
using CartEntity = Organi.Server.Domain.Entities.Cart;

namespace Organi.Server.UnitTests.Features.Orders;

public sealed class CreateOrderHandlerTests
{
    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
    private readonly ILogger<CreateOrderHandler> _logger = Substitute.For<ILogger<CreateOrderHandler>>();
    private readonly CreateOrderHandler _handler;

    public CreateOrderHandlerTests()
    {
        _handler = new CreateOrderHandler(_context, _currentUser, _logger);
    }

    private static CreateOrderCommand ValidCommand() => new(
        "Jane", "Doe", "123 Main St", "Springfield", "12345", "555-0100", "jane@example.com", null);

    private void SetupCarts(params CartEntity[] carts)
    {
        var mockSet = carts.ToList().BuildMockDbSet();
        _context.Carts.Returns(mockSet);
    }

    private void SetupOrders(params Order[] orders)
    {
        var mockSet = orders.ToList().BuildMockDbSet();
        _context.Orders.Returns(mockSet);
    }

    [Fact]
    public async Task Handle_NoCart_ThrowsBusinessRuleException()
    {
        _currentUser.UserId.Returns(Guid.NewGuid());
        SetupCarts();

        var act = () => _handler.Handle(ValidCommand(), CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*cart is empty*");
    }

    [Fact]
    public async Task Handle_InsufficientStock_ThrowsBusinessRuleException()
    {
        var userId = Guid.NewGuid();
        var vendorId = Guid.NewGuid();
        var product = new Product { Name = "Honey", Slug = "honey", SKU = "HNY-001", Price = 10m, StockQuantity = 1, VendorId = vendorId };
        var cartItem = new CartItem { Product = product, ProductId = product.Id, Quantity = 5, UnitPrice = 10m };
        var cart = new CartEntity { UserId = userId, CartItems = [cartItem] };

        _currentUser.UserId.Returns(userId);
        SetupCarts(cart);

        var act = () => _handler.Handle(ValidCommand(), CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*Insufficient stock*");
    }

    [Fact]
    public async Task Handle_ValidCart_CreatesOrderAndDoesNotDecrementStock()
    {
        var userId = Guid.NewGuid();
        var vendorId = Guid.NewGuid();
        var product = new Product { Name = "Honey", Slug = "honey", SKU = "HNY-001", Price = 10m, StockQuantity = 20, VendorId = vendorId };
        var cartItem = new CartItem { Product = product, ProductId = product.Id, Quantity = 3, UnitPrice = 10m };
        var cart = new CartEntity { UserId = userId, CartItems = [cartItem] };

        _currentUser.UserId.Returns(userId);
        SetupCarts(cart);
        SetupOrders();

        var result = await _handler.Handle(ValidCommand(), CancellationToken.None);

        result.SubTotal.Should().Be(30m);
        result.TotalAmount.Should().Be(30m);
        result.Items.Should().ContainSingle(i => i.ProductId == product.Id && i.Quantity == 3);
        product.StockQuantity.Should().Be(20, "stock is only decremented on confirm, not at checkout");
    }
}
