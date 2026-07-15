using FluentAssertions;
using Organi.Server.Application.Features.Reviews.Commands.CreateReview;
using Xunit;

namespace Organi.Server.UnitTests.Features.Reviews;

public sealed class CreateReviewValidatorTests
{
    private readonly CreateReviewValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_ReturnsSuccess()
    {
        var result = _validator.Validate(new CreateReviewCommand(5, "Great", "Loved it"));

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public void Validate_RatingOutOfRange_ReturnsError(int rating)
    {
        var result = _validator.Validate(new CreateReviewCommand(rating, null, null));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Rating");
    }

    [Fact]
    public void Validate_CommentTooLong_ReturnsError()
    {
        var result = _validator.Validate(new CreateReviewCommand(5, null, new string('a', 1001)));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Comment");
    }
}
