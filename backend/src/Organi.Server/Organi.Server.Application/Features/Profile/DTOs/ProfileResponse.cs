namespace Organi.Server.Application.Features.Profile.DTOs;

public sealed record ProfileResponse(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    DateOnly? DateOfBirth,
    string? AvatarUrl,
    bool EmailConfirmed,
    DateTime? LastLoginAt,
    IReadOnlyList<string> Roles,
    DateTime CreatedAt);
