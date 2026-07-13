using Organi.Server.Domain.Entities;

namespace Organi.Server.Application.Common.Interfaces;

public sealed record AccessToken(string Value, DateTime ExpiresAt);

public sealed record GeneratedRefreshToken(string Value, DateTime ExpiresAt);

public interface ITokenService
{
    AccessToken GenerateAccessToken(User user, IReadOnlyList<string> roles, IReadOnlyList<string> permissions, Guid? vendorId);
    GeneratedRefreshToken GenerateRefreshToken();
    string HashToken(string token);
}
