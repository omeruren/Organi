using MediatR;
using Organi.Server.Application.Common.Models;
using Organi.Server.Application.Features.AuditLogs.DTOs;
using Organi.Server.Application.Features.AuditLogs.Queries.GetAuditLogs;

namespace Organi.Server.WebAPI.Endpoints;

public static class AuditLogEndpoints
{
    public static void MapAuditLogEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/audit-logs").WithTags("AuditLogs").RequireAuthorization("IsAdmin");

        group.MapGet("/", GetAuditLogs)
            .WithName("GetAuditLogs")
            .WithDescription("Retrieves a paginated, filterable list of audit log entries.")
            .Produces<PagedResponse<AuditLogResponse>>(StatusCodes.Status200OK)
            .ProducesValidationProblem();
    }

    private static async Task<IResult> GetAuditLogs(
        ISender sender,
        CancellationToken cancellationToken,
        string? entityName = null,
        Guid? userId = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        int page = 1,
        int pageSize = 10)
    {
        var result = await sender.Send(
            new GetAuditLogsQuery(entityName, userId, fromDate, toDate, page, pageSize), cancellationToken);
        return Results.Ok(result);
    }
}
