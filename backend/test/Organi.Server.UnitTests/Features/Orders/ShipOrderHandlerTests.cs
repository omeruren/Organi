using FluentAssertions;
using Microsoft.Extensions.Logging;
using MockQueryable.NSubstitute;
using NSubstitute;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Orders.Commands.ShipOrder;
using Organi.Server.Domain.Entities;
using Organi.Server.Domain.Enums;
using Organi.Server.Domain.Exceptions;
using Xunit;

namespace Organi.Server.UnitTests.Features.Orders;

public sealed class ShipOrderHandlerTests
{
    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
    private readonly ILogger<ShipOrderHandler> _logger = Substitute.For<ILogger<ShipOrderHandler>>();
    private readonly ShipOrderHandler _handler;

    public ShipOrderHandlerTests()
    {
        _handler = new ShipOrderHandler(_context, _currentUser, _logger);
    }

    private void SetupOrders(params Order[] orders)
    {
        var mockSet = orders.ToList().BuildMockDbSet();
        _context.Orders.Returns(mockSet);
    }

    [Fact]
    public async Task Handle_OrderNotConfirmed_ThrowsBusinessRuleException()
    {
        var order = new Order { OrderNumber = "ORD-1", Status = OrderStatus.Pending, OrderItems = [] };
        SetupOrders(order);
        _currentUser.IsInRole("Admin").Returns(true);

        var act = () => _handler.Handle(new ShipOrderCommand(order.Id), CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*confirmed orders*");
    }

    [Fact]
    public async Task Handle_ConfirmedOrder_MarksAsShipped()
    {
        var order = new Order { OrderNumber = "ORD-1", Status = OrderStatus.Confirmed, OrderItems = [] };
        SetupOrders(order);
        _currentUser.IsInRole("Admin").Returns(true);

        var result = await _handler.Handle(new ShipOrderCommand(order.Id), CancellationToken.None);

        result.Status.Should().Be(nameof(OrderStatus.Shipped));
    }
}
