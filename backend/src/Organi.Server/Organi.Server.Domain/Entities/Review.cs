using Organi.Server.Domain.Common;

namespace Organi.Server.Domain.Entities;

public sealed class Review : AuditableEntity
{
    public int Rating { get; set; }
    public string? Title { get; set; }
    public string? Comment { get; set; }
    public bool IsApproved { get; set; }
    public Guid ProductId { get; set; }
    public Guid UserId { get; set; }

    public Product Product { get; set; } = default!;
    public User User { get; set; } = default!;
}
