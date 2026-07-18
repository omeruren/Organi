using FluentAssertions;
using Organi.Server.Application.Features.Reviews.Queries.GetReviews;
using Xunit;

namespace Organi.Server.UnitTests.Features.Reviews;

public sealed class GetReviewsValidatorTests
{
    private readonly GetReviewsValidator _validator = new();

    [Fact]
    public void Validate_DefaultQuery_ReturnsSuccess()
    {
        var result = _validator.Validate(new GetReviewsQuery());

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    public void Validate_RatingInRange_ReturnsSuccess(int rating)
    {
        var result = _validator.Validate(new GetReviewsQuery(Rating: rating));

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public void Validate_RatingOutOfRange_ReturnsError(int rating)
    {
        var result = _validator.Validate(new GetReviewsQuery(Rating: rating));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Rating");
    }

    [Fact]
    public void Validate_PageZero_ReturnsError()
    {
        var result = _validator.Validate(new GetReviewsQuery(Page: 0));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Page");
    }

    [Fact]
    public void Validate_PageSizeOverCap_ReturnsError()
    {
        var result = _validator.Validate(new GetReviewsQuery(PageSize: 51));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "PageSize");
    }
}
