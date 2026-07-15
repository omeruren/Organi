using FluentAssertions;
using Microsoft.Extensions.Logging;
using MockQueryable.NSubstitute;
using NSubstitute;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Orders.Commands.CancelOrder;
using Organi.Server.Domain.Entities;
using Organi.Server.Domain.Enums;
using Organi.Server.Domain.Exceptions;
using Xunit;

namespace Organi.Server.UnitTests.Features.Orders;

public sealed class CancelOrderHandlerTests
{
    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
    private readonly IAuditService _auditService = Substitute.For<IAuditService>();
    private readonly ILogger<CancelOrderHandler> _logger = Substitute.For<ILogger<CancelOrderHandler>>();
    private readonly CancelOrderHandler _handler;

    public CancelOrderHandlerTests()
    {
        _handler = new CancelOrderHandler(_context, _currentUser, _auditService, _logger);
    }

    private void SetupOrders(params Order[] orders)
    {
        var mockSet = orders.ToList().BuildMockDbSet();
        _context.Orders.Returns(mockSet);
    }

    private static (Order order, Product product) BuildOrder(Guid userId, OrderStatus status, int stock = 5, int quantity = 2)
    {
        var product = new Product { Name = "Honey", Slug = "honey", SKU = "HNY-001", StockQuantity = stock };
        var item = new OrderItem { Product = product, ProductId = product.Id, ProductName = "Honey", ProductSKU = "HNY-001", Quantity = quantity, UnitPrice = 10m, TotalPrice = 10m * quantity };
        var order = new Order { OrderNumber = "ORD-1", Status = status, UserId = userId, OrderItems = [item] };
        return (order, product);
    }

    [Fact]
    public async Task Handle_NotOwnerNotAdmin_ThrowsForbiddenException()
    {
        var (order, _) = BuildOrder(Guid.NewGuid(), OrderStatus.Pending);
        SetupOrders(order);

        _currentUser.UserId.Returns(Guid.NewGuid());
        _currentUser.IsInRole("Admin").Returns(false);

        var act = () => _handler.Handle(new CancelOrderCommand(null) { Id = order.Id }, CancellationToken.None);

        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task Handle_DeliveredOrder_ThrowsBusinessRuleException()
    {
        var userId = Guid.NewGuid();
        var (order, _) = BuildOrder(userId, OrderStatus.Delivered);
        SetupOrders(order);

        _currentUser.UserId.Returns(userId);
        _currentUser.IsInRole("Admin").Returns(false);

        var act = () => _handler.Handle(new CancelOrderCommand(null) { Id = order.Id }, CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*Delivered*");
    }

    [Fact]
    public async Task Handle_AlreadyCancelled_ThrowsBusinessRuleException()
    {
        var userId = Guid.NewGuid();
        var (order, _) = BuildOrder(userId, OrderStatus.Cancelled);
        SetupOrders(order);

        _currentUser.UserId.Returns(userId);
        _currentUser.IsInRole("Admin").Returns(false);

        var act = () => _handler.Handle(new CancelOrderCommand(null) { Id = order.Id }, CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*already cancelled*");
    }

    [Fact]
    public async Task Handle_PendingOrder_CancelsWithoutRestoringStock()
    {
        var userId = Guid.NewGuid();
        var (order, product) = BuildOrder(userId, OrderStatus.Pending, stock: 5, quantity: 2);
        SetupOrders(order);

        _currentUser.UserId.Returns(userId);
        _currentUser.IsInRole("Admin").Returns(false);

        var result = await _handler.Handle(new CancelOrderCommand("Changed my mind") { Id = order.Id }, CancellationToken.None);

        result.Status.Should().Be(nameof(OrderStatus.Cancelled));
        product.StockQuantity.Should().Be(5, "stock was never decremented for a still-Pending order");
    }

    [Fact]
    public async Task Handle_ConfirmedOrder_CancelsAndRestoresStock()
    {
        var userId = Guid.NewGuid();
        var (order, product) = BuildOrder(userId, OrderStatus.Confirmed, stock: 5, quantity: 2);
        SetupOrders(order);

        _currentUser.UserId.Returns(userId);
        _currentUser.IsInRole("Admin").Returns(false);

        var result = await _handler.Handle(new CancelOrderCommand(null) { Id = order.Id }, CancellationToken.None);

        result.Status.Should().Be(nameof(OrderStatus.Cancelled));
        product.StockQuantity.Should().Be(7, "stock committed at confirm must be restored on cancel");
    }
}
