using FluentAssertions;
using Microsoft.Extensions.Logging;
using MockQueryable.NSubstitute;
using NSubstitute;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Compare.Commands.RemoveCompareItem;
using Organi.Server.Domain.Entities;
using Organi.Server.Domain.Exceptions;
using Xunit;

namespace Organi.Server.UnitTests.Features.Compare;

public sealed class RemoveCompareItemHandlerTests
{
    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
    private readonly ILogger<RemoveCompareItemHandler> _logger = Substitute.For<ILogger<RemoveCompareItemHandler>>();
    private readonly RemoveCompareItemHandler _handler;

    public RemoveCompareItemHandlerTests()
    {
        _handler = new RemoveCompareItemHandler(_context, _currentUser, _logger);
    }

    private void SetupCompareItems(params CompareItem[] items)
    {
        var mockSet = items.ToList().BuildMockDbSet();
        _context.CompareItems.Returns(mockSet);
    }

    [Fact]
    public async Task Handle_ExistingItem_RemovesIt()
    {
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var item = new CompareItem { UserId = userId, ProductId = productId, Product = new Product { Name = "Honey", Slug = "honey", SKU = "HNY-001" } };
        SetupCompareItems(item);
        _currentUser.UserId.Returns(userId);

        await _handler.Handle(new RemoveCompareItemCommand(productId), CancellationToken.None);

        _context.CompareItems.Received(1).Remove(item);
    }

    [Fact]
    public async Task Handle_ItemNotFound_ThrowsNotFoundException()
    {
        SetupCompareItems();
        _currentUser.UserId.Returns(Guid.NewGuid());

        var act = () => _handler.Handle(new RemoveCompareItemCommand(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
