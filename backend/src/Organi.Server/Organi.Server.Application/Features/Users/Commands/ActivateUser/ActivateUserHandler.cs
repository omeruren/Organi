using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Users.DTOs;
using Organi.Server.Application.Features.Users.Mappings;
using Organi.Server.Domain.Enums;
using Organi.Server.Domain.Exceptions;

namespace Organi.Server.Application.Features.Users.Commands.ActivateUser;

public sealed class ActivateUserHandler(
    IApplicationDbContext context,
    IAuditService auditService,
    ILogger<ActivateUserHandler> logger) : IRequestHandler<ActivateUserCommand, UserResponse>
{
    public async Task<UserResponse> Handle(ActivateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await context.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("User", request.Id);

        if (user.IsActive)
            throw new BusinessRuleException("User is already active.");

        user.IsActive = true;

        auditService.Log("User", user.Id.ToString(), AuditAction.Update, newValues: new { IsActive = true });

        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("User {UserId} activated", user.Id);

        return user.ToResponse();
    }
}
