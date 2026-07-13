using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Organi.Server.Application.Common.Interfaces;

namespace Organi.Server.Application.Features.Auth.Commands.Logout;

public sealed class LogoutHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUserService,
    ILogger<LogoutHandler> logger) : IRequestHandler<LogoutCommand>
{
    public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId
            ?? throw new UnauthorizedAccessException("No authenticated user context.");

        var activeTokens = await context.RefreshTokens
            .Where(rt => rt.UserId == userId && !rt.IsRevoked)
            .ToListAsync(cancellationToken);

        foreach (var token in activeTokens)
        {
            token.IsRevoked = true;
            token.RevokedAt = DateTime.UtcNow;
        }

        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("User {UserId} logged out", userId);
    }
}
