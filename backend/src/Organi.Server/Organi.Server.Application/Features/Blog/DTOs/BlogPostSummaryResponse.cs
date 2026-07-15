namespace Organi.Server.Application.Features.Blog.DTOs;

public sealed record BlogPostSummaryResponse(
    Guid Id,
    string Title,
    string Slug,
    string? Excerpt,
    string? FeaturedImageUrl,
    bool IsPublished,
    DateTime? PublishedAt,
    string AuthorName,
    int CommentCount,
    DateTime CreatedAt);
