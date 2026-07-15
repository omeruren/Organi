using FluentAssertions;
using Organi.Server.Application.Features.Reviews.Commands.UpdateReview;
using Xunit;

namespace Organi.Server.UnitTests.Features.Reviews;

public sealed class UpdateReviewValidatorTests
{
    private readonly UpdateReviewValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_ReturnsSuccess()
    {
        var result = _validator.Validate(new UpdateReviewCommand(5, "Great", "Loved it"));

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public void Validate_RatingOutOfRange_ReturnsError(int rating)
    {
        var result = _validator.Validate(new UpdateReviewCommand(rating, null, null));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Rating");
    }
}
