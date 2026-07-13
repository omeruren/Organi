using Organi.Server.Domain.Common;

namespace Organi.Server.Domain.Entities;

public sealed class Cart : BaseEntity
{
    public Guid UserId { get; set; }

    public User User { get; set; } = default!;
    public ICollection<CartItem> CartItems { get; set; } = [];
}
