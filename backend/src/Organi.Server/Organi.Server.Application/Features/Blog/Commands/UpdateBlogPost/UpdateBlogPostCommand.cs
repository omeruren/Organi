using MediatR;
using Organi.Server.Application.Features.Blog.DTOs;

namespace Organi.Server.Application.Features.Blog.Commands.UpdateBlogPost;

public sealed record UpdateBlogPostCommand(
    string Title,
    string Content,
    string? Excerpt,
    string? FeaturedImageUrl,
    bool IsPublished) : IRequest<BlogPostResponse>
{
    public Guid Id { get; init; }
}
