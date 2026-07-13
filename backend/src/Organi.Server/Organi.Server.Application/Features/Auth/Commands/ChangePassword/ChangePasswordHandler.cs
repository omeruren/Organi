using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Domain.Entities;
using Organi.Server.Domain.Exceptions;

namespace Organi.Server.Application.Features.Auth.Commands.ChangePassword;

public sealed class ChangePasswordHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUserService,
    IPasswordHasher passwordHasher,
    ILogger<ChangePasswordHandler> logger) : IRequestHandler<ChangePasswordCommand>
{
    public async Task Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId
            ?? throw new UnauthorizedAccessException("No authenticated user context.");

        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken)
            ?? throw new NotFoundException(nameof(User), userId);

        if (!passwordHasher.Verify(request.CurrentPassword, user.PasswordHash))
            throw new BusinessRuleException("Current password is incorrect.");

        user.PasswordHash = passwordHasher.Hash(request.NewPassword);

        var activeTokens = await context.RefreshTokens
            .Where(rt => rt.UserId == userId && !rt.IsRevoked)
            .ToListAsync(cancellationToken);

        foreach (var token in activeTokens)
        {
            token.IsRevoked = true;
            token.RevokedAt = DateTime.UtcNow;
        }

        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("User {UserId} changed password", userId);
    }
}
