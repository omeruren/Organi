namespace Organi.Server.Application.Features.Reviews.DTOs;

public sealed record ReviewResponse(
    Guid Id,
    int Rating,
    string? Title,
    string? Comment,
    Guid ProductId,
    Guid UserId,
    string UserFullName,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
