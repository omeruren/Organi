// Third-party Imports
import { useQuery } from '@tanstack/react-query'

// Lib Imports
import { apiFetch } from '@/libs/api-client'

// Type Imports
import type { PagedResponse } from '@/types/api/common'
import type { AuditLogResponse } from '@/types/api/auditLog'

export interface UseAuditLogsParams {
  page: number
  pageSize: number
  entityName?: string
  fromDate?: string
  toDate?: string
}

export const useAuditLogs = (params: UseAuditLogsParams) =>
  useQuery({
    queryKey: ['audit-logs', params],
    queryFn: () => {
      const query = new URLSearchParams({ page: String(params.page), pageSize: String(params.pageSize) })

      if (params.entityName) query.set('entityName', params.entityName)
      if (params.fromDate) query.set('fromDate', params.fromDate)
      if (params.toDate) query.set('toDate', params.toDate)

      return apiFetch<PagedResponse<AuditLogResponse>>(`/api/audit-logs?${query}`)
    }
  })
