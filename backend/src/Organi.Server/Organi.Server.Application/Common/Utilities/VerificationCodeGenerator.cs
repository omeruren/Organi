using System.Security.Cryptography;

namespace Organi.Server.Application.Common.Utilities;

/// <summary>
/// Cryptographically secure secrets for the email-verification and password-reset flows.
/// Mirrors the RandomNumberGenerator approach already used for refresh tokens.
/// </summary>
public static class VerificationCodeGenerator
{
    /// <summary>URL-safe token for the email-confirmation link (no padding or +/ to escape).</summary>
    public static string GenerateToken() => Base64UrlEncode(RandomNumberGenerator.GetBytes(48));

    /// <summary>Six-digit numeric code for password reset, zero-padded (e.g. "004271").</summary>
    public static string GenerateNumericCode() => RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");

    private static string Base64UrlEncode(byte[] bytes) =>
        Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
}
