using Organi.Server.Domain.Common;

namespace Organi.Server.Domain.Entities;

public sealed class ContactMessage : AuditableEntity
{
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Subject { get; set; } = default!;
    public string Message { get; set; } = default!;
    public bool IsHandled { get; set; }
}
