namespace Organi.Server.Application.Features.Auth.DTOs;

public sealed record AuthResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt);
