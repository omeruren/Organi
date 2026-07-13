using Organi.Server.Domain.Common;

namespace Organi.Server.Domain.Entities;

public sealed class BlogComment : BaseEntity
{
    public string Content { get; set; } = default!;
    public bool IsApproved { get; set; }
    public Guid BlogPostId { get; set; }
    public Guid UserId { get; set; }

    public BlogPost BlogPost { get; set; } = default!;
    public User User { get; set; } = default!;
}
