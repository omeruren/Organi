using FluentAssertions;
using Microsoft.Extensions.Logging;
using MockQueryable.NSubstitute;
using NSubstitute;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Orders.Commands.DeliverOrder;
using Organi.Server.Domain.Entities;
using Organi.Server.Domain.Enums;
using Organi.Server.Domain.Exceptions;
using Xunit;

namespace Organi.Server.UnitTests.Features.Orders;

public sealed class DeliverOrderHandlerTests
{
    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly ILogger<DeliverOrderHandler> _logger = Substitute.For<ILogger<DeliverOrderHandler>>();
    private readonly DeliverOrderHandler _handler;

    public DeliverOrderHandlerTests()
    {
        _handler = new DeliverOrderHandler(_context, _logger);
    }

    private void SetupOrders(params Order[] orders)
    {
        var mockSet = orders.ToList().BuildMockDbSet();
        _context.Orders.Returns(mockSet);
    }

    [Fact]
    public async Task Handle_OrderNotShipped_ThrowsBusinessRuleException()
    {
        var order = new Order { OrderNumber = "ORD-1", Status = OrderStatus.Confirmed, OrderItems = [] };
        SetupOrders(order);

        var act = () => _handler.Handle(new DeliverOrderCommand(order.Id), CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*shipped orders*");
    }

    [Fact]
    public async Task Handle_ShippedOrder_MarksAsDelivered()
    {
        var order = new Order { OrderNumber = "ORD-1", Status = OrderStatus.Shipped, OrderItems = [] };
        SetupOrders(order);

        var result = await _handler.Handle(new DeliverOrderCommand(order.Id), CancellationToken.None);

        result.Status.Should().Be(nameof(OrderStatus.Delivered));
    }
}
