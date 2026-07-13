using Organi.Server.Domain.Entities;

namespace Organi.Server.Persistence.Configurations;

internal static class RoleSeedData
{
    public static readonly Guid AdminId = new("10000000-0000-0000-0000-000000000001");
    public static readonly Guid VendorId = new("10000000-0000-0000-0000-000000000002");
    public static readonly Guid CustomerId = new("10000000-0000-0000-0000-000000000003");

    private static readonly DateTime SeedTimestamp = new(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc);

    public static readonly Role[] Roles =
    [
        new Role
        {
            Id = AdminId,
            Name = "Admin",
            Description = "Full system access",
            IsSystemRole = true,
            CreatedAt = SeedTimestamp
        },
        new Role
        {
            Id = VendorId,
            Name = "Vendor",
            Description = "Vendor operations and product management",
            IsSystemRole = true,
            CreatedAt = SeedTimestamp
        },
        new Role
        {
            Id = CustomerId,
            Name = "Customer",
            Description = "Standard customer access",
            IsSystemRole = true,
            CreatedAt = SeedTimestamp
        }
    ];
}
