using Organi.Server.Domain.Common;

namespace Organi.Server.Domain.Entities;

public sealed class WishlistItem : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid ProductId { get; set; }

    public User User { get; set; } = default!;
    public Product Product { get; set; } = default!;
}
