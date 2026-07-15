namespace Organi.Server.Application.Features.Blog.DTOs;

public sealed record BlogPostResponse(
    Guid Id,
    string Title,
    string Slug,
    string Content,
    string? Excerpt,
    string? FeaturedImageUrl,
    bool IsPublished,
    DateTime? PublishedAt,
    int ViewCount,
    Guid AuthorId,
    string AuthorName,
    int CommentCount,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
