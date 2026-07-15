using FluentAssertions;
using MockQueryable.NSubstitute;
using NSubstitute;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Reports.Queries.GetSalesReport;
using Organi.Server.Domain.Entities;
using Organi.Server.Domain.Enums;
using Xunit;

namespace Organi.Server.UnitTests.Features.Reports;

public sealed class GetSalesReportHandlerTests
{
    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly GetSalesReportHandler _handler;

    public GetSalesReportHandlerTests()
    {
        _handler = new GetSalesReportHandler(_context);
    }

    private void SetupOrders(params Order[] orders)
    {
        var mockSet = orders.ToList().BuildMockDbSet();
        _context.Orders.Returns(mockSet);
    }

    [Fact]
    public async Task Handle_OnlyQualifyingStatusesCounted()
    {
        var confirmed = new Order { OrderNumber = "1", Status = OrderStatus.Confirmed, TotalAmount = 100m };
        var pending = new Order { OrderNumber = "2", Status = OrderStatus.Pending, TotalAmount = 50m };
        var cancelled = new Order { OrderNumber = "3", Status = OrderStatus.Cancelled, TotalAmount = 999m };
        SetupOrders(confirmed, pending, cancelled);

        var result = await _handler.Handle(new GetSalesReportQuery(), CancellationToken.None);

        result.TotalRevenue.Should().Be(100m);
        result.TotalOrders.Should().Be(1);
        result.AverageOrderValue.Should().Be(100m);
    }

    [Fact]
    public async Task Handle_NoQualifyingOrders_ReturnsZeroes()
    {
        SetupOrders();

        var result = await _handler.Handle(new GetSalesReportQuery(), CancellationToken.None);

        result.TotalRevenue.Should().Be(0m);
        result.TotalOrders.Should().Be(0);
        result.AverageOrderValue.Should().Be(0m);
    }

    [Fact]
    public async Task Handle_DateRangeFilter_ExcludesOutOfRange()
    {
        var inRange = new Order { OrderNumber = "1", Status = OrderStatus.Delivered, TotalAmount = 100m, CreatedAt = DateTime.UtcNow };
        var tooOld = new Order { OrderNumber = "2", Status = OrderStatus.Delivered, TotalAmount = 200m, CreatedAt = DateTime.UtcNow.AddDays(-30) };
        SetupOrders(inRange, tooOld);

        var result = await _handler.Handle(new GetSalesReportQuery(FromDate: DateTime.UtcNow.AddDays(-1)), CancellationToken.None);

        result.TotalRevenue.Should().Be(100m);
        result.TotalOrders.Should().Be(1);
    }
}
