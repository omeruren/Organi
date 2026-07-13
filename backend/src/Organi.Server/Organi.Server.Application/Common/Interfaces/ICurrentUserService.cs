namespace Organi.Server.Application.Common.Interfaces;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    string? Email { get; }
    IReadOnlyList<string> Roles { get; }
    Guid? VendorId { get; }
    bool IsInRole(string role);
}
