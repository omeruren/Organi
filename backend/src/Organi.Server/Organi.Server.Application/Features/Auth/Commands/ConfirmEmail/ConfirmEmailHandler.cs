using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Domain.Exceptions;

namespace Organi.Server.Application.Features.Auth.Commands.ConfirmEmail;

public sealed class ConfirmEmailHandler(
    IApplicationDbContext context,
    ITokenService tokenService,
    ILogger<ConfirmEmailHandler> logger) : IRequestHandler<ConfirmEmailCommand>
{
    public async Task Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        // Only the hash is stored, so the incoming token is hashed and matched on that.
        var tokenHash = tokenService.HashToken(request.Token);

        var user = await context.Users
            .FirstOrDefaultAsync(u => u.EmailConfirmationTokenHash == tokenHash, cancellationToken)
            ?? throw new BusinessRuleException("This confirmation link is invalid or has already been used.");

        if (user.EmailConfirmationTokenExpiresAt is null || user.EmailConfirmationTokenExpiresAt < DateTime.UtcNow)
            throw new BusinessRuleException("This confirmation link has expired. Please request a new one.");

        user.EmailConfirmed = true;

        // Single-use: clearing the hash makes a replay of the same link fail above.
        user.EmailConfirmationTokenHash = null;
        user.EmailConfirmationTokenExpiresAt = null;

        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("User {UserId} confirmed their email address", user.Id);
    }
}
