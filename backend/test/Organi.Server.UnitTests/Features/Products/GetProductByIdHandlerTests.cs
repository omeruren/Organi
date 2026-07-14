using FluentAssertions;
using MockQueryable.NSubstitute;
using NSubstitute;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Products.Queries.GetProductById;
using Organi.Server.Domain.Entities;
using Organi.Server.Domain.Enums;
using Organi.Server.Domain.Exceptions;
using Xunit;

namespace Organi.Server.UnitTests.Features.Products;

public sealed class GetProductByIdHandlerTests
{
    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
    private readonly GetProductByIdHandler _handler;

    public GetProductByIdHandlerTests()
    {
        _handler = new GetProductByIdHandler(_context, _currentUser);
    }

    private void SetupProducts(params Product[] products)
    {
        var mockSet = products.ToList().BuildMockDbSet();
        _context.Products.Returns(mockSet);
    }

    private static Product BuildProduct(Guid id, ProductStatus status, VendorStatus vendorStatus, Guid vendorId) => new()
    {
        Id = id,
        Name = "Organic Honey",
        Slug = "organic-honey",
        SKU = "HNY-001",
        Status = status,
        VendorId = vendorId,
        Vendor = new Vendor { Id = vendorId, StoreName = "Farm", Status = vendorStatus },
        Category = new Category { Id = Guid.NewGuid(), Name = "Honey" },
        ProductImages = []
    };

    [Fact]
    public async Task Handle_ProductNotFound_ThrowsNotFoundException()
    {
        SetupProducts();

        var act = () => _handler.Handle(new GetProductByIdQuery(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_DraftProductAsAnonymous_ThrowsNotFoundException()
    {
        var productId = Guid.NewGuid();
        var product = BuildProduct(productId, ProductStatus.Draft, VendorStatus.Approved, Guid.NewGuid());

        SetupProducts(product);
        _currentUser.IsInRole("Admin").Returns(false);
        _currentUser.VendorId.Returns((Guid?)null);

        var act = () => _handler.Handle(new GetProductByIdQuery(productId), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_DraftProductAsOwningVendor_ReturnsProduct()
    {
        var productId = Guid.NewGuid();
        var vendorId = Guid.NewGuid();
        var product = BuildProduct(productId, ProductStatus.Draft, VendorStatus.Approved, vendorId);

        SetupProducts(product);
        _currentUser.IsInRole("Admin").Returns(false);
        _currentUser.VendorId.Returns(vendorId);

        var result = await _handler.Handle(new GetProductByIdQuery(productId), CancellationToken.None);

        result.Id.Should().Be(productId);
    }

    [Fact]
    public async Task Handle_ActiveProductFromApprovedVendor_ReturnsProduct()
    {
        var productId = Guid.NewGuid();
        var product = BuildProduct(productId, ProductStatus.Active, VendorStatus.Approved, Guid.NewGuid());

        SetupProducts(product);
        _currentUser.IsInRole("Admin").Returns(false);
        _currentUser.VendorId.Returns((Guid?)null);

        var result = await _handler.Handle(new GetProductByIdQuery(productId), CancellationToken.None);

        result.Id.Should().Be(productId);
    }
}
