namespace Organi.Server.Persistence.Configurations;

internal static class RolePermissionSeedData
{
    private static readonly Guid[] AdminPermissions =
    [
        PermissionSeedData.ProductsCreate, PermissionSeedData.ProductsRead, PermissionSeedData.ProductsUpdate, PermissionSeedData.ProductsDelete,
        PermissionSeedData.OrdersCreate, PermissionSeedData.OrdersRead, PermissionSeedData.OrdersUpdate, PermissionSeedData.OrdersCancel,
        PermissionSeedData.CategoriesManage, PermissionSeedData.VendorsRead, PermissionSeedData.VendorsManage,
        PermissionSeedData.UsersRead, PermissionSeedData.UsersManage, PermissionSeedData.CouponsManage,
        PermissionSeedData.ReviewsManage, PermissionSeedData.BlogCreate, PermissionSeedData.BlogManage,
        PermissionSeedData.ReportsView, PermissionSeedData.AuditLogsView
    ];

    private static readonly Guid[] VendorPermissions =
    [
        PermissionSeedData.ProductsCreate, PermissionSeedData.ProductsRead, PermissionSeedData.ProductsUpdate, PermissionSeedData.ProductsDelete,
        PermissionSeedData.OrdersRead, PermissionSeedData.OrdersUpdate, PermissionSeedData.BlogCreate
    ];

    private static readonly Guid[] CustomerPermissions =
    [
        PermissionSeedData.ProductsRead, PermissionSeedData.OrdersCreate, PermissionSeedData.OrdersRead, PermissionSeedData.OrdersCancel
    ];

    public static readonly Dictionary<string, object>[] Mappings =
    [
        .. Map(RoleSeedData.AdminId, AdminPermissions),
        .. Map(RoleSeedData.VendorId, VendorPermissions),
        .. Map(RoleSeedData.CustomerId, CustomerPermissions)
    ];

    private static IEnumerable<Dictionary<string, object>> Map(Guid roleId, Guid[] permissionIds) =>
        permissionIds.Select(permissionId => new Dictionary<string, object>
        {
            ["RoleId"] = roleId,
            ["PermissionId"] = permissionId
        });
}
