using System.Text.Json;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Domain.Entities;
using Organi.Server.Domain.Enums;

namespace Organi.Server.Application.Common.Services;

public sealed class AuditService(
    IApplicationDbContext context,
    ICurrentUserService currentUser) : IAuditService
{
    public void Log(string entityName, string entityId, AuditAction action, object? oldValues = null, object? newValues = null)
    {
        context.AuditLogs.Add(new AuditLog
        {
            EntityName = entityName,
            EntityId = entityId,
            Action = action,
            OldValues = oldValues is null ? null : JsonSerializer.Serialize(oldValues),
            NewValues = newValues is null ? null : JsonSerializer.Serialize(newValues),
            UserId = currentUser.UserId,
            UserEmail = currentUser.Email,
            Timestamp = DateTime.UtcNow,
            IpAddress = currentUser.IpAddress
        });
    }
}
