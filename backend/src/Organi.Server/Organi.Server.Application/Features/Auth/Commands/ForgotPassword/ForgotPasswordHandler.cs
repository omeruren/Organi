using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Common.Utilities;

namespace Organi.Server.Application.Features.Auth.Commands.ForgotPassword;

public sealed class ForgotPasswordHandler(
    IApplicationDbContext context,
    ITokenService tokenService,
    IEmailService emailService,
    ILogger<ForgotPasswordHandler> logger) : IRequestHandler<ForgotPasswordCommand>
{
    public async Task Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        // Unknown or deactivated accounts fall through silently — the endpoint returns the same
        // 204 either way, so it can't be used to enumerate registered email addresses.
        if (user is null || !user.IsActive)
        {
            logger.LogInformation("Password reset requested for {Email} — no action taken", request.Email);

            return;
        }

        var code = VerificationCodeGenerator.GenerateNumericCode();

        user.PasswordResetCodeHash = tokenService.HashToken(code);
        user.PasswordResetCodeExpiresAt = DateTime.UtcNow.Add(AuthVerificationPolicy.PasswordResetCodeLifetime);
        user.PasswordResetAttemptCount = 0;

        await context.SaveChangesAsync(cancellationToken);

        // Post-commit so the code is guaranteed persisted before the email quoting it goes out,
        // and CancellationToken.None so a disconnect can't strand the user without their code.
        await emailService.SendPasswordResetCodeAsync(user.Email, $"{user.FirstName} {user.LastName}", code, CancellationToken.None);

        logger.LogInformation("Password reset code issued for user {UserId}", user.Id);
    }
}
