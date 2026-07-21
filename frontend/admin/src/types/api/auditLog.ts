// Mirrors Organi.Server.Domain.Enums.AuditAction (serialized as string)
export type AuditAction =
  | 'Create'
  | 'Update'
  | 'Delete'
  | 'Login'
  | 'Logout'
  | 'FailedLogin'
  | 'PasswordChange'
  | 'RoleAssigned'
  | 'RoleRemoved'
  | 'OrderPlaced'
  | 'OrderCancelled'
  | 'VendorApproved'
  | 'VendorSuspended'

// Mirrors Organi.Server.Application.Features.AuditLogs.DTOs.AuditLogResponse
export interface AuditLogResponse {
  id: string
  entityName: string
  entityId: string
  action: AuditAction
  oldValues: string | null
  newValues: string | null
  userId: string | null
  userEmail: string | null
  timestamp: string
  ipAddress: string | null
}
