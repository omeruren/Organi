namespace Organi.Server.Application.Features.Reviews.DTOs;

public sealed record AdminReviewResponse(
    Guid Id,
    int Rating,
    string? Title,
    string? Comment,
    Guid ProductId,
    string ProductName,
    Guid UserId,
    string UserFullName,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
