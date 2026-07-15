using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Organi.Server.Application.Common.Interfaces;

namespace Organi.Server.Infrastructure.Services;

public sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

    public Guid? UserId
    {
        get
        {
            var value = User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? User?.FindFirstValue("sub");
            return Guid.TryParse(value, out var id) ? id : null;
        }
    }

    public string? Email => User?.FindFirstValue(ClaimTypes.Email);

    public IReadOnlyList<string> Roles =>
        User?.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList() ?? [];

    public Guid? VendorId
    {
        get
        {
            var value = User?.FindFirstValue("vendor_id");
            return Guid.TryParse(value, out var id) ? id : null;
        }
    }

    public string? IpAddress => httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();

    public bool IsInRole(string role) => User?.IsInRole(role) ?? false;
}
