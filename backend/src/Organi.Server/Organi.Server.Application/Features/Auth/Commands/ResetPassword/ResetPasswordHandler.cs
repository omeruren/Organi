using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Domain.Enums;
using Organi.Server.Domain.Exceptions;

namespace Organi.Server.Application.Features.Auth.Commands.ResetPassword;

public sealed class ResetPasswordHandler(
    IApplicationDbContext context,
    IPasswordHasher passwordHasher,
    ITokenService tokenService,
    IAuditService auditService,
    ILogger<ResetPasswordHandler> logger) : IRequestHandler<ResetPasswordCommand>
{
    private const string InvalidMessage = "This reset code is invalid or has expired. Please request a new one.";

    public async Task Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        // One message for every failure mode below, so a caller can't distinguish "no such
        // account" from "wrong code".
        if (user?.PasswordResetCodeHash is null || user.PasswordResetCodeExpiresAt is null)
            throw new BusinessRuleException(InvalidMessage);

        if (user.PasswordResetCodeExpiresAt < DateTime.UtcNow)
            throw new BusinessRuleException(InvalidMessage);

        if (user.PasswordResetAttemptCount >= AuthVerificationPolicy.MaxPasswordResetAttempts)
        {
            ClearResetCode(user);
            await context.SaveChangesAsync(cancellationToken);

            logger.LogWarning("Password reset for user {UserId} burned after too many attempts", user.Id);

            throw new BusinessRuleException("Too many incorrect attempts. Please request a new reset code.");
        }

        if (user.PasswordResetCodeHash != tokenService.HashToken(request.Code))
        {
            // Persist the failed guess — otherwise a 6-digit code could be brute-forced.
            user.PasswordResetAttemptCount++;
            await context.SaveChangesAsync(cancellationToken);

            throw new BusinessRuleException(InvalidMessage);
        }

        user.PasswordHash = passwordHasher.Hash(request.NewPassword);
        ClearResetCode(user);

        // A reset is a credential change: drop every existing session, mirroring ChangePassword.
        var activeTokens = await context.RefreshTokens
            .Where(rt => rt.UserId == user.Id && !rt.IsRevoked)
            .ToListAsync(cancellationToken);

        foreach (var token in activeTokens)
        {
            token.IsRevoked = true;
            token.RevokedAt = DateTime.UtcNow;
        }

        // Reaching an emailed code proves control of the inbox, so treat the address as verified.
        user.EmailConfirmed = true;

        // Let the user back in immediately — a forgotten password is the usual reason an
        // account gets locked out in the first place.
        user.FailedLoginCount = 0;
        user.LockoutEnd = null;

        auditService.Log("User", user.Id.ToString(), AuditAction.PasswordChange);

        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("User {UserId} reset their password; {TokenCount} session(s) revoked", user.Id, activeTokens.Count);
    }

    private static void ClearResetCode(Domain.Entities.User user)
    {
        user.PasswordResetCodeHash = null;
        user.PasswordResetCodeExpiresAt = null;
        user.PasswordResetAttemptCount = 0;
    }
}
