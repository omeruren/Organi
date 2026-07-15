using FluentValidation;

namespace Organi.Server.Application.Features.Blog.Commands.UpdateBlogPost;

public sealed class UpdateBlogPostValidator : AbstractValidator<UpdateBlogPostCommand>
{
    public UpdateBlogPostValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required.")
            .MaximumLength(10000).WithMessage("Content must not exceed 10000 characters.");

        RuleFor(x => x.Excerpt)
            .MaximumLength(500).WithMessage("Excerpt must not exceed 500 characters.");

        RuleFor(x => x.FeaturedImageUrl)
            .MaximumLength(500).WithMessage("Featured image URL must not exceed 500 characters.");
    }
}
