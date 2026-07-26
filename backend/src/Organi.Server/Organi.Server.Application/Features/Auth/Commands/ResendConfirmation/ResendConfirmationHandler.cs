using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Common.Utilities;

namespace Organi.Server.Application.Features.Auth.Commands.ResendConfirmation;

public sealed class ResendConfirmationHandler(
    IApplicationDbContext context,
    ITokenService tokenService,
    IEmailService emailService,
    ILogger<ResendConfirmationHandler> logger) : IRequestHandler<ResendConfirmationCommand>
{
    public async Task Handle(ResendConfirmationCommand request, CancellationToken cancellationToken)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        // Silent no-op for unknown or already-confirmed addresses: the endpoint always reports
        // success so it cannot be used to discover which emails have accounts.
        if (user is null || user.EmailConfirmed)
        {
            logger.LogInformation("Confirmation resend requested for {Email} — no action taken", request.Email);

            return;
        }

        var token = VerificationCodeGenerator.GenerateToken();

        user.EmailConfirmationTokenHash = tokenService.HashToken(token);
        user.EmailConfirmationTokenExpiresAt = DateTime.UtcNow.Add(AuthVerificationPolicy.EmailConfirmationLifetime);

        await context.SaveChangesAsync(cancellationToken);

        // After the commit, and with CancellationToken.None: the token is now live, so the mail
        // carrying it must go out even if the client has disconnected.
        await emailService.SendEmailConfirmationAsync(user.Email, $"{user.FirstName} {user.LastName}", token, CancellationToken.None);

        logger.LogInformation("Confirmation email re-sent to user {UserId}", user.Id);
    }
}
