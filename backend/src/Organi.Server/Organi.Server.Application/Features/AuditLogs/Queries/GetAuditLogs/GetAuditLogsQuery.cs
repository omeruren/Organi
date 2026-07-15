using MediatR;
using Organi.Server.Application.Common.Models;
using Organi.Server.Application.Features.AuditLogs.DTOs;

namespace Organi.Server.Application.Features.AuditLogs.Queries.GetAuditLogs;

public sealed record GetAuditLogsQuery(
    string? EntityName = null,
    Guid? UserId = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null,
    int Page = 1,
    int PageSize = 10) : IRequest<PagedResponse<AuditLogResponse>>;
