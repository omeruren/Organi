using FluentValidation;

namespace Organi.Server.Application.Features.Reviews.Queries.GetReviews;

public sealed class GetReviewsValidator : AbstractValidator<GetReviewsQuery>
{
    public GetReviewsValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("Page must be at least 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 50).WithMessage("Page size must be between 1 and 50.");

        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5)
            .When(x => x.Rating.HasValue)
            .WithMessage("Rating must be between 1 and 5.");
    }
}
