using FluentValidation.TestHelper;
using Organi.Server.Application.Features.Vendors.Queries.GetVendors;
using Xunit;

namespace Organi.Server.UnitTests.Features.Vendors;

public sealed class GetVendorsValidatorTests
{
    private readonly GetVendorsValidator _validator = new();

    [Fact]
    public void InvalidStatus_Fails()
    {
        var result = _validator.TestValidate(new GetVendorsQuery(Status: "Bogus"));

        result.ShouldHaveValidationErrorFor(x => x.Status);
    }

    [Theory]
    [InlineData("Pending")]
    [InlineData("approved")]
    [InlineData("SUSPENDED")]
    public void ValidStatus_AnyCasing_Passes(string status)
    {
        var result = _validator.TestValidate(new GetVendorsQuery(Status: status));

        result.ShouldNotHaveValidationErrorFor(x => x.Status);
    }

    [Fact]
    public void NullStatus_Passes()
    {
        var result = _validator.TestValidate(new GetVendorsQuery());

        result.ShouldNotHaveValidationErrorFor(x => x.Status);
    }

    [Fact]
    public void PageBelowOne_Fails()
    {
        var result = _validator.TestValidate(new GetVendorsQuery(Page: 0));

        result.ShouldHaveValidationErrorFor(x => x.Page);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(51)]
    public void PageSizeOutOfBounds_Fails(int pageSize)
    {
        var result = _validator.TestValidate(new GetVendorsQuery(PageSize: pageSize));

        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }
}
