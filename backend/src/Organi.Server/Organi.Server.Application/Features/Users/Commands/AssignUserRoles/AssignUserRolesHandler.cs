using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Users.DTOs;
using Organi.Server.Application.Features.Users.Mappings;
using Organi.Server.Domain.Enums;
using Organi.Server.Domain.Exceptions;

namespace Organi.Server.Application.Features.Users.Commands.AssignUserRoles;

public sealed class AssignUserRolesHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser,
    IAuditService auditService,
    ILogger<AssignUserRolesHandler> logger) : IRequestHandler<AssignUserRolesCommand, UserResponse>
{
    private const string AdminRoleName = "Admin";

    public async Task<UserResponse> Handle(AssignUserRolesCommand request, CancellationToken cancellationToken)
    {
        var user = await context.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("User", request.Id);

        var requestedNames = request.RoleNames.Distinct().ToList();

        var targetRoles = await context.Roles
            .Where(r => requestedNames.Contains(r.Name))
            .ToListAsync(cancellationToken);

        if (targetRoles.Count != requestedNames.Count)
            throw new BusinessRuleException("One or more roles do not exist.");

        var oldRoleNames = user.Roles.Select(r => r.Name).ToHashSet();
        var newRoleNames = targetRoles.Select(r => r.Name).ToHashSet();

        if (user.Id == currentUser.UserId
            && oldRoleNames.Contains(AdminRoleName)
            && !newRoleNames.Contains(AdminRoleName))
            throw new BusinessRuleException("You cannot remove your own Admin role.");

        var addedRoleNames = newRoleNames.Except(oldRoleNames).ToList();
        var removedRoleNames = oldRoleNames.Except(newRoleNames).ToList();

        user.Roles.Clear();
        foreach (var role in targetRoles)
            user.Roles.Add(role);

        foreach (var roleName in addedRoleNames)
            auditService.Log("User", user.Id.ToString(), AuditAction.RoleAssigned, newValues: new { Role = roleName });

        foreach (var roleName in removedRoleNames)
            auditService.Log("User", user.Id.ToString(), AuditAction.RoleRemoved, oldValues: new { Role = roleName });

        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Roles for user {UserId} updated by {AdminUserId}", user.Id, currentUser.UserId);

        return user.ToResponse();
    }
}
