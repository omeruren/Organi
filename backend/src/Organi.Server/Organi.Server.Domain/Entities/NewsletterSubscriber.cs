using Organi.Server.Domain.Common;

namespace Organi.Server.Domain.Entities;

public sealed class NewsletterSubscriber : AuditableEntity
{
    public string Email { get; set; } = default!;
    public bool IsConfirmed { get; set; }
    public DateTime SubscribedAt { get; set; }
    public DateTime? UnsubscribedAt { get; set; }
    public bool IsActive { get; set; }
}
