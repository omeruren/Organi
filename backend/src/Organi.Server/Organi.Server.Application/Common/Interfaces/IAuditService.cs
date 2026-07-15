using Organi.Server.Domain.Enums;

namespace Organi.Server.Application.Common.Interfaces;

public interface IAuditService
{
    /// <summary>
    /// Stages an audit log entry on the current change tracker. The caller's own
    /// SaveChangesAsync persists it atomically alongside the business change it describes.
    /// </summary>
    void Log(string entityName, string entityId, AuditAction action, object? oldValues = null, object? newValues = null);
}
