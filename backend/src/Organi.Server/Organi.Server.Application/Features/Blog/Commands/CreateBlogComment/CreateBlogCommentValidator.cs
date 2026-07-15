using FluentValidation;

namespace Organi.Server.Application.Features.Blog.Commands.CreateBlogComment;

public sealed class CreateBlogCommentValidator : AbstractValidator<CreateBlogCommentCommand>
{
    public CreateBlogCommentValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required.")
            .MaximumLength(1000).WithMessage("Content must not exceed 1000 characters.");
    }
}
