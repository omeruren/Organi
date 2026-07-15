using FluentAssertions;
using MockQueryable.NSubstitute;
using NSubstitute;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Reports.Queries.GetOrdersReport;
using Organi.Server.Domain.Entities;
using Organi.Server.Domain.Enums;
using Xunit;

namespace Organi.Server.UnitTests.Features.Reports;

public sealed class GetOrdersReportHandlerTests
{
    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly GetOrdersReportHandler _handler;

    public GetOrdersReportHandlerTests()
    {
        _handler = new GetOrdersReportHandler(_context);
    }

    private void SetupOrders(params Order[] orders)
    {
        var mockSet = orders.ToList().BuildMockDbSet();
        _context.Orders.Returns(mockSet);
    }

    [Fact]
    public async Task Handle_GroupsCountsByStatus()
    {
        var orders = new[]
        {
            new Order { OrderNumber = "1", Status = OrderStatus.Pending },
            new Order { OrderNumber = "2", Status = OrderStatus.Pending },
            new Order { OrderNumber = "3", Status = OrderStatus.Delivered },
            new Order { OrderNumber = "4", Status = OrderStatus.Cancelled }
        };
        SetupOrders(orders);

        var result = await _handler.Handle(new GetOrdersReportQuery(), CancellationToken.None);

        result.TotalOrders.Should().Be(4);
        result.Pending.Should().Be(2);
        result.Delivered.Should().Be(1);
        result.Cancelled.Should().Be(1);
        result.Confirmed.Should().Be(0);
    }
}
