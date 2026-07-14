using FluentAssertions;
using Organi.Server.Application.Features.Products.Commands.CreateProduct;
using Organi.Server.Application.Features.Products.DTOs;
using Xunit;

namespace Organi.Server.UnitTests.Features.Products;

public sealed class CreateProductValidatorTests
{
    private readonly CreateProductValidator _validator = new();

    private static CreateProductCommand ValidCommand(
        decimal price = 12.50m,
        decimal? salePrice = null,
        int stockQuantity = 100,
        IReadOnlyList<ProductImageRequest>? images = null) => new(
            "Organic Honey",
            "Raw wildflower honey.",
            "Raw honey",
            price,
            salePrice,
            "HNY-001",
            stockQuantity,
            "jar",
            500m,
            true,
            false,
            Guid.NewGuid(),
            images);

    [Fact]
    public void Validate_ValidCommand_ReturnsSuccess()
    {
        var result = _validator.Validate(ValidCommand());

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_EmptyName_ReturnsError()
    {
        var command = ValidCommand() with { Name = "" };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_InvalidPrice_ReturnsError(decimal price)
    {
        var result = _validator.Validate(ValidCommand(price: price));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Price");
    }

    [Fact]
    public void Validate_SalePriceNotLessThanPrice_ReturnsError()
    {
        var result = _validator.Validate(ValidCommand(price: 10m, salePrice: 12m));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "SalePrice");
    }

    [Fact]
    public void Validate_NegativeStockQuantity_ReturnsError()
    {
        var result = _validator.Validate(ValidCommand(stockQuantity: -1));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "StockQuantity");
    }

    [Fact]
    public void Validate_MultiplePrimaryImages_ReturnsError()
    {
        var images = new List<ProductImageRequest>
        {
            new("https://example.com/1.jpg", null, 0, true),
            new("https://example.com/2.jpg", null, 1, true)
        };

        var result = _validator.Validate(ValidCommand(images: images));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Images");
    }
}
