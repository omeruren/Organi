using Organi.Server.Domain.Entities;

namespace Organi.Server.Persistence.Configurations;

internal static class PermissionSeedData
{
    private static readonly DateTime SeedTimestamp = new(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc);

    public static readonly Guid ProductsCreate = new("20000000-0000-0000-0000-000000000001");
    public static readonly Guid ProductsRead = new("20000000-0000-0000-0000-000000000002");
    public static readonly Guid ProductsUpdate = new("20000000-0000-0000-0000-000000000003");
    public static readonly Guid ProductsDelete = new("20000000-0000-0000-0000-000000000004");
    public static readonly Guid OrdersCreate = new("20000000-0000-0000-0000-000000000005");
    public static readonly Guid OrdersRead = new("20000000-0000-0000-0000-000000000006");
    public static readonly Guid OrdersUpdate = new("20000000-0000-0000-0000-000000000007");
    public static readonly Guid OrdersCancel = new("20000000-0000-0000-0000-000000000008");
    public static readonly Guid CategoriesManage = new("20000000-0000-0000-0000-000000000009");
    public static readonly Guid VendorsRead = new("20000000-0000-0000-0000-000000000010");
    public static readonly Guid VendorsManage = new("20000000-0000-0000-0000-000000000011");
    public static readonly Guid UsersRead = new("20000000-0000-0000-0000-000000000012");
    public static readonly Guid UsersManage = new("20000000-0000-0000-0000-000000000013");
    public static readonly Guid CouponsManage = new("20000000-0000-0000-0000-000000000014");
    public static readonly Guid ReviewsManage = new("20000000-0000-0000-0000-000000000015");
    public static readonly Guid BlogCreate = new("20000000-0000-0000-0000-000000000016");
    public static readonly Guid BlogManage = new("20000000-0000-0000-0000-000000000017");
    public static readonly Guid ReportsView = new("20000000-0000-0000-0000-000000000018");
    public static readonly Guid AuditLogsView = new("20000000-0000-0000-0000-000000000019");

    public static readonly Permission[] Permissions =
    [
        Create(ProductsCreate, "Products.Create", "Create new products", "Products"),
        Create(ProductsRead, "Products.Read", "View product information", "Products"),
        Create(ProductsUpdate, "Products.Update", "Update product information", "Products"),
        Create(ProductsDelete, "Products.Delete", "Delete products", "Products"),
        Create(OrdersCreate, "Orders.Create", "Create orders", "Orders"),
        Create(OrdersRead, "Orders.Read", "View orders", "Orders"),
        Create(OrdersUpdate, "Orders.Update", "Update order status", "Orders"),
        Create(OrdersCancel, "Orders.Cancel", "Cancel orders", "Orders"),
        Create(CategoriesManage, "Categories.Manage", "Manage categories", "Categories"),
        Create(VendorsRead, "Vendors.Read", "View vendor information", "Vendors"),
        Create(VendorsManage, "Vendors.Manage", "Manage vendor accounts", "Vendors"),
        Create(UsersRead, "Users.Read", "View user information", "Users"),
        Create(UsersManage, "Users.Manage", "Manage user accounts", "Users"),
        Create(CouponsManage, "Coupons.Manage", "Manage coupons", "Coupons"),
        Create(ReviewsManage, "Reviews.Manage", "Manage reviews", "Reviews"),
        Create(BlogCreate, "Blog.Create", "Create blog posts", "Blog"),
        Create(BlogManage, "Blog.Manage", "Manage all blog posts", "Blog"),
        Create(ReportsView, "Reports.View", "View system reports", "Reports"),
        Create(AuditLogsView, "AuditLogs.View", "View audit logs", "AuditLogs")
    ];

    private static Permission Create(Guid id, string name, string description, string module) => new()
    {
        Id = id,
        Name = name,
        Description = description,
        Module = module,
        CreatedAt = SeedTimestamp
    };
}
