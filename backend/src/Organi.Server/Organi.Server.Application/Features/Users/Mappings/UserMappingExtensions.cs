using Organi.Server.Application.Features.Users.DTOs;
using Organi.Server.Domain.Entities;

namespace Organi.Server.Application.Features.Users.Mappings;

public static class UserMappingExtensions
{
    public static UserResponse ToResponse(this User user) => new(
        user.Id,
        user.Email,
        user.FirstName,
        user.LastName,
        user.PhoneNumber,
        user.IsActive,
        user.EmailConfirmed,
        user.LastLoginAt,
        user.Roles.Select(r => r.Name).ToList(),
        user.CreatedAt);
}
