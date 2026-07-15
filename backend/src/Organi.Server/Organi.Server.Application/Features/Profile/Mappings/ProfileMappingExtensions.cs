using Organi.Server.Application.Features.Profile.DTOs;
using Organi.Server.Domain.Entities;

namespace Organi.Server.Application.Features.Profile.Mappings;

public static class ProfileMappingExtensions
{
    public static ProfileResponse ToResponse(this User user) => new(
        user.Id,
        user.Email,
        user.FirstName,
        user.LastName,
        user.PhoneNumber,
        user.DateOfBirth,
        user.AvatarUrl,
        user.EmailConfirmed,
        user.LastLoginAt,
        user.Roles.Select(r => r.Name).ToList(),
        user.CreatedAt);
}
