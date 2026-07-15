using FluentAssertions;
using Microsoft.Extensions.Logging;
using MockQueryable.NSubstitute;
using NSubstitute;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Compare.Commands.AddCompareItem;
using Organi.Server.Domain.Entities;
using Organi.Server.Domain.Exceptions;
using Xunit;

namespace Organi.Server.UnitTests.Features.Compare;

public sealed class AddCompareItemHandlerTests
{
    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
    private readonly ILogger<AddCompareItemHandler> _logger = Substitute.For<ILogger<AddCompareItemHandler>>();
    private readonly AddCompareItemHandler _handler;

    public AddCompareItemHandlerTests()
    {
        _handler = new AddCompareItemHandler(_context, _currentUser, _logger);
    }

    private void SetupProducts(params Product[] products)
    {
        var mockSet = products.ToList().BuildMockDbSet();
        _context.Products.Returns(mockSet);
    }

    private void SetupCompareItems(params CompareItem[] items)
    {
        var mockSet = items.ToList().BuildMockDbSet();
        _context.CompareItems.Returns(mockSet);
    }

    [Fact]
    public async Task Handle_ProductNotCompared_AddsNewItem()
    {
        var userId = Guid.NewGuid();
        var product = new Product { Name = "Honey", Slug = "honey", SKU = "HNY-001", Price = 5m };
        SetupProducts(product);
        SetupCompareItems();
        _currentUser.UserId.Returns(userId);

        var result = await _handler.Handle(new AddCompareItemCommand(product.Id), CancellationToken.None);

        result.ProductId.Should().Be(product.Id);
    }

    [Fact]
    public async Task Handle_ProductAlreadyCompared_ReturnsExistingWithoutError()
    {
        var userId = Guid.NewGuid();
        var product = new Product { Name = "Honey", Slug = "honey", SKU = "HNY-001", Price = 5m };
        var existing = new CompareItem { UserId = userId, ProductId = product.Id, Product = product };
        SetupProducts(product);
        SetupCompareItems(existing);
        _currentUser.UserId.Returns(userId);

        var result = await _handler.Handle(new AddCompareItemCommand(product.Id), CancellationToken.None);

        result.Id.Should().Be(existing.Id);
    }

    [Fact]
    public async Task Handle_ProductNotFound_ThrowsNotFoundException()
    {
        SetupProducts();
        SetupCompareItems();
        _currentUser.UserId.Returns(Guid.NewGuid());

        var act = () => _handler.Handle(new AddCompareItemCommand(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
