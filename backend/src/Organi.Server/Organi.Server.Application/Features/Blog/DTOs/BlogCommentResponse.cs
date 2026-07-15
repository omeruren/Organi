namespace Organi.Server.Application.Features.Blog.DTOs;

public sealed record BlogCommentResponse(
    Guid Id,
    string Content,
    Guid BlogPostId,
    Guid UserId,
    string UserFullName,
    DateTime CreatedAt);
