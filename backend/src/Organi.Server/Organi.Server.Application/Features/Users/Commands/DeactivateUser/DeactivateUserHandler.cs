using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Users.DTOs;
using Organi.Server.Application.Features.Users.Mappings;
using Organi.Server.Domain.Enums;
using Organi.Server.Domain.Exceptions;

namespace Organi.Server.Application.Features.Users.Commands.DeactivateUser;

public sealed class DeactivateUserHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser,
    IAuditService auditService,
    ILogger<DeactivateUserHandler> logger) : IRequestHandler<DeactivateUserCommand, UserResponse>
{
    public async Task<UserResponse> Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await context.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("User", request.Id);

        if (user.Id == currentUser.UserId)
            throw new BusinessRuleException("You cannot deactivate your own account.");

        if (!user.IsActive)
            throw new BusinessRuleException("User is already inactive.");

        user.IsActive = false;

        auditService.Log("User", user.Id.ToString(), AuditAction.Update, newValues: new { IsActive = false });

        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("User {UserId} deactivated by {AdminUserId}", user.Id, currentUser.UserId);

        return user.ToResponse();
    }
}
