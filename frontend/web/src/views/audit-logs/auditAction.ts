// Type Imports
import type { ChipProps } from '@mui/material/Chip'

import type { AuditAction } from '@/types/api/auditLog'

export const auditActionColorMap: Record<AuditAction, ChipProps['color']> = {
  Create: 'success',
  Update: 'info',
  Delete: 'error',
  Login: 'default',
  Logout: 'default',
  FailedLogin: 'error',
  PasswordChange: 'default',
  RoleAssigned: 'success',
  RoleRemoved: 'error',
  OrderPlaced: 'success',
  OrderCancelled: 'error',
  VendorApproved: 'success',
  VendorSuspended: 'error'
}

// Entities that are actually audit-logged by the backend (auditService.Log calls).
export const AUDIT_ENTITY_OPTIONS = ['Order', 'User', 'Vendor']
