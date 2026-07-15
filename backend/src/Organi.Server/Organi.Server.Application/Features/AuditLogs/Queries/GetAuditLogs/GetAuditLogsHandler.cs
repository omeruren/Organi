using MediatR;
using Microsoft.EntityFrameworkCore;
using Organi.Server.Application.Common.Extensions;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Common.Models;
using Organi.Server.Application.Features.AuditLogs.DTOs;

namespace Organi.Server.Application.Features.AuditLogs.Queries.GetAuditLogs;

public sealed class GetAuditLogsHandler(
    IApplicationDbContext context) : IRequestHandler<GetAuditLogsQuery, PagedResponse<AuditLogResponse>>
{
    public async Task<PagedResponse<AuditLogResponse>> Handle(GetAuditLogsQuery request, CancellationToken cancellationToken)
    {
        var query = context.AuditLogs.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.EntityName))
            query = query.Where(a => a.EntityName == request.EntityName);

        if (request.UserId.HasValue)
            query = query.Where(a => a.UserId == request.UserId.Value);

        if (request.FromDate.HasValue)
            query = query.Where(a => a.Timestamp >= request.FromDate.Value);

        if (request.ToDate.HasValue)
            query = query.Where(a => a.Timestamp <= request.ToDate.Value);

        var projected = query
            .OrderByDescending(a => a.Timestamp)
            .Select(a => new AuditLogResponse(
                a.Id,
                a.EntityName,
                a.EntityId,
                a.Action.ToString(),
                a.OldValues,
                a.NewValues,
                a.UserId,
                a.UserEmail,
                a.Timestamp,
                a.IpAddress));

        return await projected.ToPagedResponseAsync(request.Page, request.PageSize, cancellationToken);
    }
}
