using FluentAssertions;
using MockQueryable.NSubstitute;
using NSubstitute;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Reports.Queries.GetVendorsReport;
using Organi.Server.Domain.Entities;
using Organi.Server.Domain.Enums;
using Xunit;

namespace Organi.Server.UnitTests.Features.Reports;

public sealed class GetVendorsReportHandlerTests
{
    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly GetVendorsReportHandler _handler;

    public GetVendorsReportHandlerTests()
    {
        _handler = new GetVendorsReportHandler(_context);
    }

    private void SetupOrderItems(params OrderItem[] items)
    {
        var mockSet = items.ToList().BuildMockDbSet();
        _context.OrderItems.Returns(mockSet);
    }

    [Fact]
    public async Task Handle_OrdersByRevenueDescending_ExcludingNonQualifyingOrders()
    {
        var vendorA = new Vendor { StoreName = "Farm A", Slug = "farm-a" };
        var vendorB = new Vendor { StoreName = "Farm B", Slug = "farm-b" };

        var qualifyingOrderA = new Order { OrderNumber = "1", Status = OrderStatus.Delivered };
        var qualifyingOrderB = new Order { OrderNumber = "2", Status = OrderStatus.Confirmed };
        var cancelledOrder = new Order { OrderNumber = "3", Status = OrderStatus.Cancelled };

        var items = new[]
        {
            new OrderItem { VendorId = vendorA.Id, Vendor = vendorA, Order = qualifyingOrderA, OrderId = qualifyingOrderA.Id, TotalPrice = 50m, ProductName = "X", ProductSKU = "X" },
            new OrderItem { VendorId = vendorB.Id, Vendor = vendorB, Order = qualifyingOrderB, OrderId = qualifyingOrderB.Id, TotalPrice = 200m, ProductName = "Y", ProductSKU = "Y" },
            new OrderItem { VendorId = vendorA.Id, Vendor = vendorA, Order = cancelledOrder, OrderId = cancelledOrder.Id, TotalPrice = 999m, ProductName = "Z", ProductSKU = "Z" }
        };
        SetupOrderItems(items);

        var result = await _handler.Handle(new GetVendorsReportQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
        result[0].StoreName.Should().Be("Farm B");
        result[0].TotalRevenue.Should().Be(200m);
        result[1].StoreName.Should().Be("Farm A");
        result[1].TotalRevenue.Should().Be(50m, "the cancelled order's revenue must not count");
    }

    [Fact]
    public async Task Handle_TopLimitsResultCount()
    {
        var vendors = Enumerable.Range(1, 5).Select(i => new Vendor { StoreName = $"Farm {i}", Slug = $"farm-{i}" }).ToList();
        var items = vendors.Select((v, i) =>
        {
            var order = new Order { OrderNumber = $"ORD-{i}", Status = OrderStatus.Delivered };
            return new OrderItem { VendorId = v.Id, Vendor = v, Order = order, OrderId = order.Id, TotalPrice = (i + 1) * 10m, ProductName = "P", ProductSKU = "P" };
        }).ToArray();
        SetupOrderItems(items);

        var result = await _handler.Handle(new GetVendorsReportQuery(Top: 2), CancellationToken.None);

        result.Should().HaveCount(2);
    }
}
