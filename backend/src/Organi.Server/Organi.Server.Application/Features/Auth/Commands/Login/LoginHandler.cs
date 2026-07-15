using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Common.Models;
using Organi.Server.Application.Features.Auth.DTOs;
using Organi.Server.Domain.Entities;
using Organi.Server.Domain.Enums;

namespace Organi.Server.Application.Features.Auth.Commands.Login;

public sealed class LoginHandler(
    IApplicationDbContext context,
    IPasswordHasher passwordHasher,
    ITokenService tokenService,
    IAuditService auditService,
    ILogger<LoginHandler> logger) : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    private const int MaxFailedAttempts = 5;
    private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(30);

    public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await context.Users
            .Include(u => u.Roles).ThenInclude(r => r.Permissions)
            .Include(u => u.Vendor)
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (user is null)
        {
            logger.LogWarning("Login failed: unknown email {Email}", request.Email);
            return Result<AuthResponse>.Failure(new Error("InvalidCredentials", "Invalid email or password."));
        }

        if (!user.IsActive)
        {
            auditService.Log("User", user.Id.ToString(), AuditAction.FailedLogin, newValues: new { Reason = "AccountDeactivated" });
            await context.SaveChangesAsync(cancellationToken);

            logger.LogWarning("Login failed: deactivated account {UserId}", user.Id);
            return Result<AuthResponse>.Failure(new Error("AccountDeactivated", "This account has been deactivated."));
        }

        if (user.LockoutEnd is { } lockoutEnd)
        {
            if (lockoutEnd > DateTime.UtcNow)
            {
                auditService.Log("User", user.Id.ToString(), AuditAction.FailedLogin, newValues: new { Reason = "AccountLocked" });
                await context.SaveChangesAsync(cancellationToken);

                logger.LogWarning("Login failed: locked account {UserId}", user.Id);
                return Result<AuthResponse>.Failure(new Error("AccountLocked", "Account is locked due to too many failed login attempts. Try again later."));
            }

            user.FailedLoginCount = 0;
            user.LockoutEnd = null;
        }

        if (!passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            user.FailedLoginCount++;

            if (user.FailedLoginCount >= MaxFailedAttempts)
            {
                user.LockoutEnd = DateTime.UtcNow.Add(LockoutDuration);
                logger.LogWarning("Account {UserId} locked after {Count} failed attempts", user.Id, user.FailedLoginCount);
            }

            auditService.Log("User", user.Id.ToString(), AuditAction.FailedLogin, newValues: new { Reason = "InvalidPassword" });

            await context.SaveChangesAsync(cancellationToken);

            logger.LogWarning("Login failed: invalid password for {UserId}", user.Id);
            return Result<AuthResponse>.Failure(new Error("InvalidCredentials", "Invalid email or password."));
        }

        user.FailedLoginCount = 0;
        user.LockoutEnd = null;
        user.LastLoginAt = DateTime.UtcNow;

        auditService.Log("User", user.Id.ToString(), AuditAction.Login);

        var roles = user.Roles.Select(r => r.Name).ToArray();
        var permissions = user.Roles.SelectMany(r => r.Permissions).Select(p => p.Name).Distinct().ToArray();
        var vendorId = user.Vendor?.Id;

        var accessToken = tokenService.GenerateAccessToken(user, roles, permissions, vendorId);
        var refreshToken = tokenService.GenerateRefreshToken();

        context.RefreshTokens.Add(new RefreshToken
        {
            TokenHash = tokenService.HashToken(refreshToken.Value),
            ExpiresAt = refreshToken.ExpiresAt,
            UserId = user.Id
        });

        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("User {UserId} logged in", user.Id);

        return Result<AuthResponse>.Success(new AuthResponse(accessToken.Value, refreshToken.Value, accessToken.ExpiresAt));
    }
}
