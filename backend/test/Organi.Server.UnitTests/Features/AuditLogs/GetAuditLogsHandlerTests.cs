using FluentAssertions;
using MockQueryable.NSubstitute;
using NSubstitute;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.AuditLogs.Queries.GetAuditLogs;
using Organi.Server.Domain.Entities;
using Organi.Server.Domain.Enums;
using Xunit;

namespace Organi.Server.UnitTests.Features.AuditLogs;

public sealed class GetAuditLogsHandlerTests
{
    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly GetAuditLogsHandler _handler;

    public GetAuditLogsHandlerTests()
    {
        _handler = new GetAuditLogsHandler(_context);
    }

    private void SetupAuditLogs(params AuditLog[] logs)
    {
        var mockSet = logs.ToList().BuildMockDbSet();
        _context.AuditLogs.Returns(mockSet);
    }

    [Fact]
    public async Task Handle_EntityNameFilter_ReturnsOnlyMatching()
    {
        var userLog = new AuditLog { EntityName = "User", EntityId = "1", Action = AuditAction.Login, Timestamp = DateTime.UtcNow };
        var orderLog = new AuditLog { EntityName = "Order", EntityId = "2", Action = AuditAction.OrderPlaced, Timestamp = DateTime.UtcNow };
        SetupAuditLogs(userLog, orderLog);

        var result = await _handler.Handle(new GetAuditLogsQuery(EntityName: "User"), CancellationToken.None);

        result.Items.Should().ContainSingle();
        result.Items[0].EntityName.Should().Be("User");
    }

    [Fact]
    public async Task Handle_UserIdFilter_ReturnsOnlyMatching()
    {
        var userId = Guid.NewGuid();
        var matching = new AuditLog { EntityName = "User", EntityId = "1", Action = AuditAction.Login, UserId = userId, Timestamp = DateTime.UtcNow };
        var other = new AuditLog { EntityName = "User", EntityId = "2", Action = AuditAction.Login, UserId = Guid.NewGuid(), Timestamp = DateTime.UtcNow };
        SetupAuditLogs(matching, other);

        var result = await _handler.Handle(new GetAuditLogsQuery(UserId: userId), CancellationToken.None);

        result.Items.Should().ContainSingle();
    }

    [Fact]
    public async Task Handle_DateRangeFilter_ExcludesOutOfRange()
    {
        var inRange = new AuditLog { EntityName = "User", EntityId = "1", Action = AuditAction.Login, Timestamp = DateTime.UtcNow };
        var tooOld = new AuditLog { EntityName = "User", EntityId = "2", Action = AuditAction.Login, Timestamp = DateTime.UtcNow.AddDays(-30) };
        SetupAuditLogs(inRange, tooOld);

        var result = await _handler.Handle(
            new GetAuditLogsQuery(FromDate: DateTime.UtcNow.AddDays(-1)), CancellationToken.None);

        result.Items.Should().ContainSingle();
    }

    [Fact]
    public async Task Handle_MultipleEntries_OrdersByMostRecentFirst()
    {
        var older = new AuditLog { EntityName = "User", EntityId = "1", Action = AuditAction.Login, Timestamp = DateTime.UtcNow.AddHours(-2) };
        var newer = new AuditLog { EntityName = "User", EntityId = "2", Action = AuditAction.Login, Timestamp = DateTime.UtcNow };
        SetupAuditLogs(older, newer);

        var result = await _handler.Handle(new GetAuditLogsQuery(), CancellationToken.None);

        result.Items[0].EntityId.Should().Be("2");
        result.Items[1].EntityId.Should().Be("1");
    }
}
