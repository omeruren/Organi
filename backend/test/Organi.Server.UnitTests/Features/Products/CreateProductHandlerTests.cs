using FluentAssertions;
using Microsoft.Extensions.Logging;
using MockQueryable.NSubstitute;
using NSubstitute;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Products.Commands.CreateProduct;
using Organi.Server.Application.Features.Products.DTOs;
using Organi.Server.Domain.Entities;
using Organi.Server.Domain.Enums;
using Organi.Server.Domain.Exceptions;
using Xunit;

namespace Organi.Server.UnitTests.Features.Products;

public sealed class CreateProductHandlerTests
{
    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
    private readonly ILogger<CreateProductHandler> _logger = Substitute.For<ILogger<CreateProductHandler>>();
    private readonly CreateProductHandler _handler;

    public CreateProductHandlerTests()
    {
        _handler = new CreateProductHandler(_context, _currentUser, _logger);
    }

    private static CreateProductCommand ValidCommand(
        Guid categoryId,
        bool isFeatured = false,
        IReadOnlyList<ProductImageRequest>? images = null) => new(
            "Organic Honey",
            "Raw wildflower honey.",
            "Raw honey",
            12.50m,
            null,
            "HNY-001",
            100,
            "jar",
            500m,
            true,
            isFeatured,
            categoryId,
            images);

    private static Vendor ApprovedVendor(Guid id) => new() { Id = id, StoreName = "Green Valley Farm", Status = VendorStatus.Approved };

    private static Category ActiveCategory(Guid id) => new() { Id = id, Name = "Honey", Slug = "honey", IsActive = true };

    private void SetupVendors(params Vendor[] vendors)
    {
        var mockSet = vendors.ToList().BuildMockDbSet();
        _context.Vendors.Returns(mockSet);
    }

    private void SetupCategories(params Category[] categories)
    {
        var mockSet = categories.ToList().BuildMockDbSet();
        _context.Categories.Returns(mockSet);
    }

    private void SetupProducts(params Product[] products)
    {
        var mockSet = products.ToList().BuildMockDbSet();
        _context.Products.Returns(mockSet);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsProductResponse()
    {
        var vendorId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        _currentUser.VendorId.Returns(vendorId);

        SetupVendors(ApprovedVendor(vendorId));
        SetupCategories(ActiveCategory(categoryId));
        SetupProducts();

        var result = await _handler.Handle(ValidCommand(categoryId), CancellationToken.None);

        result.Name.Should().Be("Organic Honey");
        result.Slug.Should().Be("organic-honey");
        result.VendorName.Should().Be("Green Valley Farm");
        result.CategoryName.Should().Be("Honey");
    }

    [Fact]
    public async Task Handle_CallerHasNoVendorProfile_ThrowsBusinessRuleException()
    {
        _currentUser.VendorId.Returns((Guid?)null);

        var act = () => _handler.Handle(ValidCommand(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*Only vendors*");
    }

    [Fact]
    public async Task Handle_VendorNotApproved_ThrowsBusinessRuleException()
    {
        var vendorId = Guid.NewGuid();
        _currentUser.VendorId.Returns(vendorId);

        SetupVendors(new Vendor { Id = vendorId, StoreName = "New Farm", Status = VendorStatus.Pending });

        var act = () => _handler.Handle(ValidCommand(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*approved vendors*");
    }

    [Fact]
    public async Task Handle_CategoryInactive_ThrowsBusinessRuleException()
    {
        var vendorId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        _currentUser.VendorId.Returns(vendorId);

        SetupVendors(ApprovedVendor(vendorId));
        SetupCategories(new Category { Id = categoryId, Name = "Honey", Slug = "honey", IsActive = false });

        var act = () => _handler.Handle(ValidCommand(categoryId), CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*active category*");
    }

    [Fact]
    public async Task Handle_DuplicateSku_ThrowsBusinessRuleException()
    {
        var vendorId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        _currentUser.VendorId.Returns(vendorId);

        SetupVendors(ApprovedVendor(vendorId));
        SetupCategories(ActiveCategory(categoryId));
        SetupProducts(new Product { SKU = "HNY-001", VendorId = Guid.NewGuid(), Name = "Other Honey" });

        var act = () => _handler.Handle(ValidCommand(categoryId), CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*SKU*");
    }

    [Fact]
    public async Task Handle_FeaturedWithoutImages_ThrowsBusinessRuleException()
    {
        var vendorId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        _currentUser.VendorId.Returns(vendorId);

        SetupVendors(ApprovedVendor(vendorId));
        SetupCategories(ActiveCategory(categoryId));
        SetupProducts();

        var act = () => _handler.Handle(ValidCommand(categoryId, isFeatured: true, images: []), CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*at least one image*");
    }
}
