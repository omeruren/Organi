using MediatR;
using Organi.Server.Application.Features.Blog.DTOs;

namespace Organi.Server.Application.Features.Blog.Commands.CreateBlogPost;

public sealed record CreateBlogPostCommand(
    string Title,
    string Content,
    string? Excerpt,
    string? FeaturedImageUrl,
    bool IsPublished) : IRequest<BlogPostResponse>;
