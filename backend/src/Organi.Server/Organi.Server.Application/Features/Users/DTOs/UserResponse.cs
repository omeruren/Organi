namespace Organi.Server.Application.Features.Users.DTOs;

public sealed record UserResponse(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    bool IsActive,
    bool EmailConfirmed,
    DateTime? LastLoginAt,
    IReadOnlyList<string> Roles,
    DateTime CreatedAt);
