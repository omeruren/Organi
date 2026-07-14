using FluentAssertions;
using Microsoft.Extensions.Logging;
using MockQueryable.NSubstitute;
using NSubstitute;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Products.Commands.UpdateProduct;
using Organi.Server.Domain.Entities;
using Organi.Server.Domain.Enums;
using Organi.Server.Domain.Exceptions;
using Xunit;

namespace Organi.Server.UnitTests.Features.Products;

public sealed class UpdateProductHandlerTests
{
    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
    private readonly ILogger<UpdateProductHandler> _logger = Substitute.For<ILogger<UpdateProductHandler>>();
    private readonly UpdateProductHandler _handler;

    public UpdateProductHandlerTests()
    {
        _handler = new UpdateProductHandler(_context, _currentUser, _logger);
    }

    private static UpdateProductCommand ValidCommand(Guid id, Guid categoryId) => new(
        "Organic Honey Updated",
        null,
        null,
        15.00m,
        null,
        "HNY-001",
        50,
        "jar",
        null,
        true,
        false,
        nameof(ProductStatus.Active),
        categoryId,
        null)
    { Id = id };

    private static Category ActiveCategory(Guid id) => new() { Id = id, Name = "Honey", Slug = "honey", IsActive = true };

    private void SetupProducts(params Product[] products)
    {
        var mockSet = products.ToList().BuildMockDbSet();
        _context.Products.Returns(mockSet);
    }

    private void SetupCategories(params Category[] categories)
    {
        var mockSet = categories.ToList().BuildMockDbSet();
        _context.Categories.Returns(mockSet);
    }

    [Fact]
    public async Task Handle_ProductNotFound_ThrowsNotFoundException()
    {
        SetupProducts();

        var act = () => _handler.Handle(ValidCommand(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_NotOwnerNotAdmin_ThrowsForbiddenException()
    {
        var productId = Guid.NewGuid();
        var ownerVendorId = Guid.NewGuid();
        var callerVendorId = Guid.NewGuid();

        var product = new Product
        {
            Id = productId,
            Name = "Organic Honey",
            Slug = "organic-honey",
            SKU = "HNY-001",
            VendorId = ownerVendorId,
            Vendor = new Vendor { Id = ownerVendorId, StoreName = "Farm" },
            Category = new Category { Id = Guid.NewGuid(), Name = "Honey", IsActive = true },
            ProductImages = []
        };

        SetupProducts(product);
        _currentUser.VendorId.Returns(callerVendorId);
        _currentUser.IsInRole("Admin").Returns(false);

        var act = () => _handler.Handle(ValidCommand(productId, product.CategoryId), CancellationToken.None);

        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task Handle_DuplicateSkuFromAnotherProduct_ThrowsBusinessRuleException()
    {
        var productId = Guid.NewGuid();
        var vendorId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();

        var product = new Product
        {
            Id = productId,
            Name = "Organic Honey",
            Slug = "organic-honey",
            SKU = "HNY-002",
            VendorId = vendorId,
            Vendor = new Vendor { Id = vendorId, StoreName = "Farm" },
            CategoryId = categoryId,
            Category = ActiveCategory(categoryId),
            ProductImages = []
        };
        var otherProduct = new Product { Id = Guid.NewGuid(), SKU = "HNY-001", Name = "Other", VendorId = Guid.NewGuid() };

        SetupProducts(product, otherProduct);
        SetupCategories(ActiveCategory(categoryId));
        _currentUser.VendorId.Returns(vendorId);
        _currentUser.IsInRole("Admin").Returns(false);

        var act = () => _handler.Handle(ValidCommand(productId, categoryId), CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*SKU*");
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsUpdatedProductResponse()
    {
        var productId = Guid.NewGuid();
        var vendorId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();

        var product = new Product
        {
            Id = productId,
            Name = "Organic Honey",
            Slug = "organic-honey",
            SKU = "HNY-001",
            VendorId = vendorId,
            Vendor = new Vendor { Id = vendorId, StoreName = "Farm" },
            CategoryId = categoryId,
            Category = ActiveCategory(categoryId),
            ProductImages = []
        };

        SetupProducts(product);
        SetupCategories(ActiveCategory(categoryId));
        _currentUser.VendorId.Returns(vendorId);
        _currentUser.IsInRole("Admin").Returns(false);

        var result = await _handler.Handle(ValidCommand(productId, categoryId), CancellationToken.None);

        result.Name.Should().Be("Organic Honey Updated");
        result.Price.Should().Be(15.00m);
        result.Status.Should().Be(nameof(ProductStatus.Active));
    }
}
