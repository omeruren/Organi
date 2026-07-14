using FluentAssertions;
using MockQueryable.NSubstitute;
using NSubstitute;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Orders.Queries.GetOrderById;
using Organi.Server.Domain.Entities;
using Organi.Server.Domain.Exceptions;
using Xunit;

namespace Organi.Server.UnitTests.Features.Orders;

public sealed class GetOrderByIdHandlerTests
{
    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
    private readonly GetOrderByIdHandler _handler;

    public GetOrderByIdHandlerTests()
    {
        _handler = new GetOrderByIdHandler(_context, _currentUser);
    }

    private void SetupOrders(params Order[] orders)
    {
        var mockSet = orders.ToList().BuildMockDbSet();
        _context.Orders.Returns(mockSet);
    }

    [Fact]
    public async Task Handle_OrderNotFound_ThrowsNotFoundException()
    {
        SetupOrders();

        var act = () => _handler.Handle(new GetOrderByIdQuery(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_UnrelatedCaller_ThrowsNotFoundException()
    {
        var order = new Order { OrderNumber = "ORD-1", UserId = Guid.NewGuid(), OrderItems = [] };
        SetupOrders(order);

        _currentUser.IsInRole("Admin").Returns(false);
        _currentUser.UserId.Returns(Guid.NewGuid());
        _currentUser.VendorId.Returns((Guid?)null);

        var act = () => _handler.Handle(new GetOrderByIdQuery(order.Id), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_Owner_ReturnsOrder()
    {
        var userId = Guid.NewGuid();
        var order = new Order { OrderNumber = "ORD-1", UserId = userId, OrderItems = [] };
        SetupOrders(order);

        _currentUser.IsInRole("Admin").Returns(false);
        _currentUser.UserId.Returns(userId);
        _currentUser.VendorId.Returns((Guid?)null);

        var result = await _handler.Handle(new GetOrderByIdQuery(order.Id), CancellationToken.None);

        result.Id.Should().Be(order.Id);
    }

    [Fact]
    public async Task Handle_ParticipatingVendor_ReturnsOrder()
    {
        var vendorId = Guid.NewGuid();
        var item = new OrderItem { ProductName = "Honey", ProductSKU = "HNY-001", VendorId = vendorId };
        var order = new Order { OrderNumber = "ORD-1", UserId = Guid.NewGuid(), OrderItems = [item] };
        SetupOrders(order);

        _currentUser.IsInRole("Admin").Returns(false);
        _currentUser.UserId.Returns(Guid.NewGuid());
        _currentUser.VendorId.Returns(vendorId);

        var result = await _handler.Handle(new GetOrderByIdQuery(order.Id), CancellationToken.None);

        result.Id.Should().Be(order.Id);
    }
}
