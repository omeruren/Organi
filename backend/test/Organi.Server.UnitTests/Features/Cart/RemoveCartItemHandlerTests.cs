using FluentAssertions;
using Microsoft.Extensions.Logging;
using MockQueryable.NSubstitute;
using NSubstitute;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Cart.Commands.RemoveCartItem;
using Organi.Server.Domain.Exceptions;
using Xunit;
using CartEntity = Organi.Server.Domain.Entities.Cart;
using CartItemEntity = Organi.Server.Domain.Entities.CartItem;

namespace Organi.Server.UnitTests.Features.Cart;

public sealed class RemoveCartItemHandlerTests
{
    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
    private readonly ILogger<RemoveCartItemHandler> _logger = Substitute.For<ILogger<RemoveCartItemHandler>>();
    private readonly RemoveCartItemHandler _handler;

    public RemoveCartItemHandlerTests()
    {
        _handler = new RemoveCartItemHandler(_context, _currentUser, _logger);
    }

    private void SetupCarts(params CartEntity[] carts)
    {
        var mockSet = carts.ToList().BuildMockDbSet();
        _context.Carts.Returns(mockSet);
    }

    [Fact]
    public async Task Handle_ItemNotInCallersCart_ThrowsNotFoundException()
    {
        var userId = Guid.NewGuid();
        _currentUser.UserId.Returns(userId);
        SetupCarts(new CartEntity { UserId = userId, CartItems = [] });

        var act = () => _handler.Handle(new RemoveCartItemCommand(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_ExistingItem_RemovesIt()
    {
        var userId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var item = new CartItemEntity { Id = itemId, Quantity = 1, UnitPrice = 10m };

        _currentUser.UserId.Returns(userId);
        SetupCarts(new CartEntity { UserId = userId, CartItems = [item] });

        await _handler.Handle(new RemoveCartItemCommand(itemId), CancellationToken.None);

        _context.CartItems.Received(1).Remove(item);
    }
}
