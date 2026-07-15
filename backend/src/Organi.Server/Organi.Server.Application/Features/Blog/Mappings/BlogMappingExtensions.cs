using Organi.Server.Application.Features.Blog.DTOs;
using Organi.Server.Domain.Entities;

namespace Organi.Server.Application.Features.Blog.Mappings;

public static class BlogMappingExtensions
{
    public static BlogPostResponse ToResponse(this BlogPost post) => new(
        post.Id,
        post.Title,
        post.Slug,
        post.Content,
        post.Excerpt,
        post.FeaturedImageUrl,
        post.IsPublished,
        post.PublishedAt,
        post.ViewCount,
        post.AuthorId,
        $"{post.Author.FirstName} {post.Author.LastName}",
        post.BlogComments.Count,
        post.CreatedAt,
        post.UpdatedAt);

    public static BlogPostSummaryResponse ToSummaryResponse(this BlogPost post) => new(
        post.Id,
        post.Title,
        post.Slug,
        post.Excerpt,
        post.FeaturedImageUrl,
        post.IsPublished,
        post.PublishedAt,
        $"{post.Author.FirstName} {post.Author.LastName}",
        post.BlogComments.Count,
        post.CreatedAt);

    public static BlogCommentResponse ToResponse(this BlogComment comment) => new(
        comment.Id,
        comment.Content,
        comment.BlogPostId,
        comment.UserId,
        $"{comment.User.FirstName} {comment.User.LastName}",
        comment.CreatedAt);
}
