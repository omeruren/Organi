using FluentAssertions;
using MockQueryable.NSubstitute;
using NSubstitute;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Orders.Queries.GetOrders;
using Organi.Server.Domain.Entities;
using Organi.Server.Domain.Enums;
using Xunit;

namespace Organi.Server.UnitTests.Features.Orders;

public sealed class GetOrdersHandlerTests
{
    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly GetOrdersHandler _handler;

    public GetOrdersHandlerTests()
    {
        _handler = new GetOrdersHandler(_context);
    }

    private void SetupOrders(params Order[] orders)
    {
        var mockSet = orders.ToList().BuildMockDbSet();
        _context.Orders.Returns(mockSet);
    }

    private static Order BuildOrder(string orderNumber, OrderStatus status, DateTime? createdAt = null)
    {
        return new Order
        {
            OrderNumber = orderNumber,
            Status = status,
            TotalAmount = 42m,
            User = new User { Email = $"{orderNumber}@organi.test", FirstName = "Ada", LastName = "Lovelace" },
            OrderItems = [],
            CreatedAt = createdAt ?? DateTime.UtcNow
        };
    }

    [Fact]
    public async Task Handle_StatusFilter_ReturnsOnlyMatching()
    {
        SetupOrders(
            BuildOrder("ORD-1", OrderStatus.Pending),
            BuildOrder("ORD-2", OrderStatus.Shipped));

        var result = await _handler.Handle(new GetOrdersQuery(Status: "pending"), CancellationToken.None);

        result.Items.Should().ContainSingle();
        result.Items[0].OrderNumber.Should().Be("ORD-1");
    }

    [Fact]
    public async Task Handle_SearchFiltersByOrderNumber()
    {
        SetupOrders(
            BuildOrder("ORD-100", OrderStatus.Pending),
            BuildOrder("ORD-200", OrderStatus.Pending));

        var result = await _handler.Handle(new GetOrdersQuery(Search: "ORD-1"), CancellationToken.None);

        result.Items.Should().ContainSingle();
        result.Items[0].OrderNumber.Should().Be("ORD-100");
    }

    [Fact]
    public async Task Handle_NoFilters_ReturnsAllOrderedByCreatedAtDesc()
    {
        SetupOrders(
            BuildOrder("ORD-OLD", OrderStatus.Pending, DateTime.UtcNow.AddDays(-2)),
            BuildOrder("ORD-NEW", OrderStatus.Delivered, DateTime.UtcNow));

        var result = await _handler.Handle(new GetOrdersQuery(), CancellationToken.None);

        result.Items.Should().HaveCount(2);
        result.Items[0].OrderNumber.Should().Be("ORD-NEW");
        result.Items[1].OrderNumber.Should().Be("ORD-OLD");
    }

    [Fact]
    public async Task Handle_ProjectsCustomerName()
    {
        SetupOrders(BuildOrder("ORD-1", OrderStatus.Pending));

        var result = await _handler.Handle(new GetOrdersQuery(), CancellationToken.None);

        result.Items[0].CustomerName.Should().Be("Ada Lovelace");
    }
}
