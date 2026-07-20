using FluentValidation.TestHelper;
using Organi.Server.Application.Features.Orders.Queries.GetOrders;
using Xunit;

namespace Organi.Server.UnitTests.Features.Orders;

public sealed class GetOrdersValidatorTests
{
    private readonly GetOrdersValidator _validator = new();

    [Fact]
    public void InvalidStatus_Fails()
    {
        var result = _validator.TestValidate(new GetOrdersQuery(Status: "Bogus"));

        result.ShouldHaveValidationErrorFor(x => x.Status);
    }

    [Theory]
    [InlineData("Pending")]
    [InlineData("shipped")]
    [InlineData("CANCELLED")]
    public void ValidStatus_AnyCasing_Passes(string status)
    {
        var result = _validator.TestValidate(new GetOrdersQuery(Status: status));

        result.ShouldNotHaveValidationErrorFor(x => x.Status);
    }

    [Fact]
    public void NullStatus_Passes()
    {
        var result = _validator.TestValidate(new GetOrdersQuery());

        result.ShouldNotHaveValidationErrorFor(x => x.Status);
    }

    [Fact]
    public void PageBelowOne_Fails()
    {
        var result = _validator.TestValidate(new GetOrdersQuery(Page: 0));

        result.ShouldHaveValidationErrorFor(x => x.Page);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(51)]
    public void PageSizeOutOfBounds_Fails(int pageSize)
    {
        var result = _validator.TestValidate(new GetOrdersQuery(PageSize: pageSize));

        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }
}
