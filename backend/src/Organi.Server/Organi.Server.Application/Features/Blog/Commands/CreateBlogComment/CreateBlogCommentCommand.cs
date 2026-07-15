using MediatR;
using Organi.Server.Application.Features.Blog.DTOs;

namespace Organi.Server.Application.Features.Blog.Commands.CreateBlogComment;

public sealed record CreateBlogCommentCommand(string Content) : IRequest<BlogCommentResponse>
{
    public Guid BlogPostId { get; init; }
}
