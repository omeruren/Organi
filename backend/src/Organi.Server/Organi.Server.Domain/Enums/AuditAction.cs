namespace Organi.Server.Domain.Enums;

public enum AuditAction
{
    Create,
    Update,
    Delete,
    Login,
    Logout,
    FailedLogin,
    PasswordChange,
    RoleAssigned,
    RoleRemoved,
    OrderPlaced,
    OrderCancelled,
    VendorApproved,
    VendorSuspended
}
