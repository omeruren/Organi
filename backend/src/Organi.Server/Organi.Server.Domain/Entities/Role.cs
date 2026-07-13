using Organi.Server.Domain.Common;

namespace Organi.Server.Domain.Entities;

public sealed class Role : BaseEntity
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public bool IsSystemRole { get; set; }

    public ICollection<User> Users { get; set; } = [];
    public ICollection<Permission> Permissions { get; set; } = [];
}
