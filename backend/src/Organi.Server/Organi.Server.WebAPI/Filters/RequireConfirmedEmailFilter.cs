using Microsoft.EntityFrameworkCore;
using Organi.Server.Application.Common.Interfaces;

namespace Organi.Server.WebAPI.Filters;

/// <summary>
/// Blocks an action when the authenticated user has not confirmed their email address.
/// Apply with <c>.AddEndpointFilter&lt;RequireConfirmedEmailFilter&gt;()</c> after <c>.RequireAuthorization()</c>.
/// </summary>
/// <remarks>
/// Reads <c>EmailConfirmed</c> from the database rather than a JWT claim on purpose: users click the
/// confirmation link from their mail client, often in a different browser, so a claim captured at login
/// would stay stale <c>false</c> for the original session with no way to refresh it.
/// </remarks>
public sealed class RequireConfirmedEmailFilter(
    IApplicationDbContext context,
    ICurrentUserService currentUser) : IEndpointFilter
{
    public const string ErrorCode = "email_not_confirmed";

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext invocationContext, EndpointFilterDelegate next)
    {
        var userId = currentUser.UserId;

        // Unauthenticated requests are already rejected by RequireAuthorization; nothing to add here.
        if (userId is null)
            return await next(invocationContext);

        var isConfirmed = await context.Users
            .Where(u => u.Id == userId.Value)
            .Select(u => u.EmailConfirmed)
            .FirstOrDefaultAsync(invocationContext.HttpContext.RequestAborted);

        if (isConfirmed)
            return await next(invocationContext);

        // Returned rather than thrown: GlobalExceptionHandler logs every exception at LogError, and an
        // unconfirmed user hitting a gated route is expected traffic, not an error worth a stack trace.
        return Results.Problem(
            statusCode: StatusCodes.Status403Forbidden,
            title: "Email Not Confirmed",
            detail: "Please confirm your email address to continue. Check your inbox for the confirmation link.",
            extensions: new Dictionary<string, object?>
            {
                ["code"] = ErrorCode,
                ["traceId"] = invocationContext.HttpContext.TraceIdentifier
            });
    }
}
