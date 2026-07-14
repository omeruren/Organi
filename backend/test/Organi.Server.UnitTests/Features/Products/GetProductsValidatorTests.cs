using FluentAssertions;
using Organi.Server.Application.Features.Products.Queries.GetProducts;
using Xunit;

namespace Organi.Server.UnitTests.Features.Products;

public sealed class GetProductsValidatorTests
{
    private readonly GetProductsValidator _validator = new();

    private static GetProductsQuery ValidQuery() => new(
        null, null, null, null, null, null, null, null, null, Page: 1, PageSize: 10);

    [Fact]
    public void Validate_ValidQuery_ReturnsSuccess()
    {
        var result = _validator.Validate(ValidQuery());

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_PageLessThanOne_ReturnsError()
    {
        var result = _validator.Validate(ValidQuery() with { Page = 0 });

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Page");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(51)]
    public void Validate_PageSizeOutOfRange_ReturnsError(int pageSize)
    {
        var result = _validator.Validate(ValidQuery() with { PageSize = pageSize });

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "PageSize");
    }

    [Fact]
    public void Validate_InvalidSortBy_ReturnsError()
    {
        var result = _validator.Validate(ValidQuery() with { SortBy = "invalid" });

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "SortBy");
    }

    [Fact]
    public void Validate_MinPriceGreaterThanMaxPrice_ReturnsError()
    {
        var result = _validator.Validate(ValidQuery() with { MinPrice = 50m, MaxPrice = 10m });

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "MinPrice");
    }
}
