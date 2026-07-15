namespace Organi.Server.Application.Features.AuditLogs.DTOs;

public sealed record AuditLogResponse(
    Guid Id,
    string EntityName,
    string EntityId,
    string Action,
    string? OldValues,
    string? NewValues,
    Guid? UserId,
    string? UserEmail,
    DateTime Timestamp,
    string? IpAddress);
