using Organi.Server.Domain.Entities;

namespace Organi.Server.Infrastructure.Security;

public interface ITokenService
{
    string GenerateAccessToken(User user, IReadOnlyList<string> roles, IReadOnlyList<string> permissions, Guid? vendorId);
    string GenerateRefreshToken();
    string HashToken(string token);
}
