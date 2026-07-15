using FluentValidation;

namespace Organi.Server.Application.Features.Newsletter.Queries.GetNewsletterSubscribers;

public sealed class GetNewsletterSubscribersValidator : AbstractValidator<GetNewsletterSubscribersQuery>
{
    public GetNewsletterSubscribersValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("Page must be at least 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 50).WithMessage("Page size must be between 1 and 50.");
    }
}
