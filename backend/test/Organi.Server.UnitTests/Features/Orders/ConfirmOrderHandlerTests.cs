using FluentAssertions;
using Microsoft.Extensions.Logging;
using MockQueryable.NSubstitute;
using NSubstitute;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Orders.Commands.ConfirmOrder;
using Organi.Server.Domain.Entities;
using Organi.Server.Domain.Enums;
using Organi.Server.Domain.Exceptions;
using Xunit;

namespace Organi.Server.UnitTests.Features.Orders;

public sealed class ConfirmOrderHandlerTests
{
    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
    private readonly ILogger<ConfirmOrderHandler> _logger = Substitute.For<ILogger<ConfirmOrderHandler>>();
    private readonly ConfirmOrderHandler _handler;

    public ConfirmOrderHandlerTests()
    {
        _handler = new ConfirmOrderHandler(_context, _currentUser, _logger);
    }

    private void SetupOrders(params Order[] orders)
    {
        var mockSet = orders.ToList().BuildMockDbSet();
        _context.Orders.Returns(mockSet);
    }

    private static (Order order, Product product) BuildPendingOrder(Guid vendorId, int stock = 10, int quantity = 2)
    {
        var product = new Product { Name = "Honey", Slug = "honey", SKU = "HNY-001", StockQuantity = stock, VendorId = vendorId };
        var orderItem = new OrderItem { Product = product, ProductId = product.Id, ProductName = "Honey", ProductSKU = "HNY-001", VendorId = vendorId, Quantity = quantity, UnitPrice = 10m, TotalPrice = 10m * quantity };
        var order = new Order { OrderNumber = "ORD-1", Status = OrderStatus.Pending, OrderItems = [orderItem] };
        return (order, product);
    }

    [Fact]
    public async Task Handle_OrderNotFound_ThrowsNotFoundException()
    {
        _currentUser.IsInRole("Admin").Returns(true);
        SetupOrders();

        var act = () => _handler.Handle(new ConfirmOrderCommand(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_VendorDoesNotOwnAllItems_ThrowsForbiddenException()
    {
        var (order, _) = BuildPendingOrder(Guid.NewGuid());
        SetupOrders(order);

        _currentUser.IsInRole("Admin").Returns(false);
        _currentUser.VendorId.Returns(Guid.NewGuid());

        var act = () => _handler.Handle(new ConfirmOrderCommand(order.Id), CancellationToken.None);

        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task Handle_AlreadyConfirmed_ThrowsBusinessRuleException()
    {
        var vendorId = Guid.NewGuid();
        var (order, _) = BuildPendingOrder(vendorId);
        order.Status = OrderStatus.Confirmed;
        SetupOrders(order);

        _currentUser.IsInRole("Admin").Returns(true);

        var act = () => _handler.Handle(new ConfirmOrderCommand(order.Id), CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*pending orders*");
    }

    [Fact]
    public async Task Handle_InsufficientStock_ThrowsBusinessRuleException()
    {
        var vendorId = Guid.NewGuid();
        var (order, _) = BuildPendingOrder(vendorId, stock: 1, quantity: 5);
        SetupOrders(order);

        _currentUser.IsInRole("Admin").Returns(true);

        var act = () => _handler.Handle(new ConfirmOrderCommand(order.Id), CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*Insufficient stock*");
    }

    [Fact]
    public async Task Handle_ValidPendingOrder_ConfirmsAndDecrementsStock()
    {
        var vendorId = Guid.NewGuid();
        var (order, product) = BuildPendingOrder(vendorId, stock: 10, quantity: 3);
        SetupOrders(order);

        _currentUser.IsInRole("Admin").Returns(false);
        _currentUser.VendorId.Returns(vendorId);

        var result = await _handler.Handle(new ConfirmOrderCommand(order.Id), CancellationToken.None);

        result.Status.Should().Be(nameof(OrderStatus.Confirmed));
        product.StockQuantity.Should().Be(7);
    }
}
