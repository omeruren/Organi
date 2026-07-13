using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Common.Models;
using Organi.Server.Application.Features.Auth.DTOs;
using Organi.Server.Domain.Entities;

namespace Organi.Server.Application.Features.Auth.Commands.Refresh;

public sealed class RefreshTokenHandler(
    IApplicationDbContext context,
    ITokenService tokenService,
    ILogger<RefreshTokenHandler> logger) : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var tokenHash = tokenService.HashToken(request.RefreshToken);

        var existingToken = await context.RefreshTokens
            .Include(rt => rt.User).ThenInclude(u => u.Roles).ThenInclude(r => r.Permissions)
            .Include(rt => rt.User).ThenInclude(u => u.Vendor)
            .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash, cancellationToken);

        if (existingToken is null)
        {
            logger.LogWarning("Refresh failed: unknown token presented");
            return Result<AuthResponse>.Failure(new Error("InvalidToken", "The refresh token is invalid."));
        }

        if (existingToken.IsRevoked)
        {
            var allUserTokens = await context.RefreshTokens
                .Where(rt => rt.UserId == existingToken.UserId && !rt.IsRevoked)
                .ToListAsync(cancellationToken);

            foreach (var token in allUserTokens)
            {
                token.IsRevoked = true;
                token.RevokedAt = DateTime.UtcNow;
            }

            await context.SaveChangesAsync(cancellationToken);

            logger.LogError("Refresh token reuse detected for user {UserId} — all sessions revoked", existingToken.UserId);
            return Result<AuthResponse>.Failure(new Error("TokenReuseDetected", "This refresh token has already been used. All sessions have been revoked."));
        }

        if (existingToken.ExpiresAt < DateTime.UtcNow)
        {
            logger.LogWarning("Refresh failed: expired token for user {UserId}", existingToken.UserId);
            return Result<AuthResponse>.Failure(new Error("TokenExpired", "The refresh token has expired."));
        }

        var user = existingToken.User;

        var roles = user.Roles.Select(r => r.Name).ToArray();
        var permissions = user.Roles.SelectMany(r => r.Permissions).Select(p => p.Name).Distinct().ToArray();
        var vendorId = user.Vendor?.Id;

        var accessToken = tokenService.GenerateAccessToken(user, roles, permissions, vendorId);
        var newRefreshToken = tokenService.GenerateRefreshToken();
        var newTokenHash = tokenService.HashToken(newRefreshToken.Value);

        existingToken.IsRevoked = true;
        existingToken.RevokedAt = DateTime.UtcNow;
        existingToken.ReplacedByTokenHash = newTokenHash;

        context.RefreshTokens.Add(new RefreshToken
        {
            TokenHash = newTokenHash,
            ExpiresAt = newRefreshToken.ExpiresAt,
            UserId = user.Id
        });

        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Refresh token rotated for user {UserId}", user.Id);

        return Result<AuthResponse>.Success(new AuthResponse(accessToken.Value, newRefreshToken.Value, accessToken.ExpiresAt));
    }
}
