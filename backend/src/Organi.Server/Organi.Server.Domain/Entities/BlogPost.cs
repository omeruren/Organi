using Organi.Server.Domain.Common;

namespace Organi.Server.Domain.Entities;

public sealed class BlogPost : AuditableEntity
{
    public string Title { get; set; } = default!;
    public string Slug { get; set; } = default!;
    public string Content { get; set; } = default!;
    public string? Excerpt { get; set; }
    public string? FeaturedImageUrl { get; set; }
    public bool IsPublished { get; set; }
    public DateTime? PublishedAt { get; set; }
    public int ViewCount { get; set; }
    public Guid AuthorId { get; set; }

    public User Author { get; set; } = default!;
    public ICollection<BlogComment> BlogComments { get; set; } = [];
}
