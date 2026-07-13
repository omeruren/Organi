using Organi.Server.Domain.Common;

namespace Organi.Server.Domain.Entities;

public sealed class Permission : BaseEntity
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public string Module { get; set; } = default!;

    public ICollection<Role> Roles { get; set; } = [];
}
