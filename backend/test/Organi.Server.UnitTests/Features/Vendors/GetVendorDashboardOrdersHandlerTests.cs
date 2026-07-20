using FluentAssertions;
using MockQueryable.NSubstitute;
using NSubstitute;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Vendors.Queries.GetVendorDashboardOrders;
using Organi.Server.Domain.Entities;
using Organi.Server.Domain.Enums;
using Organi.Server.Domain.Exceptions;
using Xunit;

namespace Organi.Server.UnitTests.Features.Vendors;

public sealed class GetVendorDashboardOrdersHandlerTests
{
    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
    private readonly GetVendorDashboardOrdersHandler _handler;

    public GetVendorDashboardOrdersHandlerTests()
    {
        _handler = new GetVendorDashboardOrdersHandler(_context, _currentUser);
    }

    private void SetupOrders(params Order[] orders)
    {
        var mockSet = orders.ToList().BuildMockDbSet();
        _context.Orders.Returns(mockSet);
    }

    private static Order BuildOrder(string orderNumber, OrderStatus status, params Guid[] itemVendorIds)
    {
        return new Order
        {
            OrderNumber = orderNumber,
            Status = status,
            TotalAmount = 42m,
            User = new User { Email = $"{orderNumber}@organi.test", FirstName = "Ada", LastName = "Lovelace" },
            OrderItems = itemVendorIds
                .Select(vendorId => new OrderItem
                {
                    ProductName = "Honey",
                    ProductSKU = "HNY-001",
                    VendorId = vendorId,
                    Quantity = 1,
                    UnitPrice = 10m,
                    TotalPrice = 10m
                })
                .ToList()
        };
    }

    [Fact]
    public async Task Handle_NoVendorProfile_ThrowsBusinessRuleException()
    {
        _currentUser.VendorId.Returns((Guid?)null);
        SetupOrders();

        var act = () => _handler.Handle(new GetVendorDashboardOrdersQuery(), CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*vendor profile*");
    }

    [Fact]
    public async Task Handle_ReturnsOnlyOrdersContainingVendorItems()
    {
        var vendorId = Guid.NewGuid();
        _currentUser.VendorId.Returns(vendorId);
        SetupOrders(
            BuildOrder("ORD-MINE", OrderStatus.Pending, vendorId),
            BuildOrder("ORD-OTHER", OrderStatus.Pending, Guid.NewGuid()));

        var result = await _handler.Handle(new GetVendorDashboardOrdersQuery(), CancellationToken.None);

        result.Items.Should().ContainSingle();
        result.Items[0].OrderNumber.Should().Be("ORD-MINE");
    }

    [Fact]
    public async Task Handle_MixedVendorOrder_IsIncluded()
    {
        var vendorId = Guid.NewGuid();
        _currentUser.VendorId.Returns(vendorId);
        SetupOrders(BuildOrder("ORD-MIXED", OrderStatus.Pending, vendorId, Guid.NewGuid()));

        var result = await _handler.Handle(new GetVendorDashboardOrdersQuery(), CancellationToken.None);

        result.Items.Should().ContainSingle();
        result.Items[0].OrderNumber.Should().Be("ORD-MIXED");
        result.Items[0].ItemCount.Should().Be(2);
    }

    [Fact]
    public async Task Handle_StatusFilter_ReturnsOnlyMatching()
    {
        var vendorId = Guid.NewGuid();
        _currentUser.VendorId.Returns(vendorId);
        SetupOrders(
            BuildOrder("ORD-PENDING", OrderStatus.Pending, vendorId),
            BuildOrder("ORD-SHIPPED", OrderStatus.Shipped, vendorId));

        var result = await _handler.Handle(new GetVendorDashboardOrdersQuery(Status: "shipped"), CancellationToken.None);

        result.Items.Should().ContainSingle();
        result.Items[0].OrderNumber.Should().Be("ORD-SHIPPED");
    }
}
