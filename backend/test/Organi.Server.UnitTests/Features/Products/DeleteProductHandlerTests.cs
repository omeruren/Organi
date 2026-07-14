using FluentAssertions;
using Microsoft.Extensions.Logging;
using MockQueryable.NSubstitute;
using NSubstitute;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Products.Commands.DeleteProduct;
using Organi.Server.Domain.Entities;
using Organi.Server.Domain.Enums;
using Organi.Server.Domain.Exceptions;
using Xunit;

namespace Organi.Server.UnitTests.Features.Products;

public sealed class DeleteProductHandlerTests
{
    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
    private readonly ILogger<DeleteProductHandler> _logger = Substitute.For<ILogger<DeleteProductHandler>>();
    private readonly DeleteProductHandler _handler;

    public DeleteProductHandlerTests()
    {
        _handler = new DeleteProductHandler(_context, _currentUser, _logger);
    }

    private void SetupProducts(params Product[] products)
    {
        var mockSet = products.ToList().BuildMockDbSet();
        _context.Products.Returns(mockSet);
    }

    private void SetupOrderItems(params OrderItem[] orderItems)
    {
        var mockSet = orderItems.ToList().BuildMockDbSet();
        _context.OrderItems.Returns(mockSet);
    }

    [Fact]
    public async Task Handle_ProductNotFound_ThrowsNotFoundException()
    {
        SetupProducts();

        var act = () => _handler.Handle(new DeleteProductCommand(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_NotOwnerNotAdmin_ThrowsForbiddenException()
    {
        var productId = Guid.NewGuid();
        var ownerVendorId = Guid.NewGuid();
        var product = new Product { Id = productId, Name = "Honey", SKU = "HNY-001", VendorId = ownerVendorId };

        SetupProducts(product);
        _currentUser.VendorId.Returns(Guid.NewGuid());
        _currentUser.IsInRole("Admin").Returns(false);

        var act = () => _handler.Handle(new DeleteProductCommand(productId), CancellationToken.None);

        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task Handle_ProductHasPendingOrders_ThrowsBusinessRuleException()
    {
        var productId = Guid.NewGuid();
        var vendorId = Guid.NewGuid();
        var product = new Product { Id = productId, Name = "Honey", SKU = "HNY-001", VendorId = vendorId };

        var order = new Order { Id = Guid.NewGuid(), OrderNumber = "ORD-1", Status = OrderStatus.Confirmed };
        var orderItem = new OrderItem { Id = Guid.NewGuid(), ProductId = productId, Order = order, VendorId = vendorId };

        SetupProducts(product);
        SetupOrderItems(orderItem);
        _currentUser.VendorId.Returns(vendorId);
        _currentUser.IsInRole("Admin").Returns(false);

        var act = () => _handler.Handle(new DeleteProductCommand(productId), CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*pending orders*");
    }

    [Fact]
    public async Task Handle_NoActiveOrders_DeletesProduct()
    {
        var productId = Guid.NewGuid();
        var vendorId = Guid.NewGuid();
        var product = new Product { Id = productId, Name = "Honey", SKU = "HNY-001", VendorId = vendorId };

        SetupProducts(product);
        SetupOrderItems();
        _currentUser.VendorId.Returns(vendorId);
        _currentUser.IsInRole("Admin").Returns(false);

        await _handler.Handle(new DeleteProductCommand(productId), CancellationToken.None);

        _context.Products.Received(1).Remove(product);
    }
}
