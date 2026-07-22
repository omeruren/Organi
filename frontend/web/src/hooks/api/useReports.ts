// Third-party Imports
import { useQuery } from '@tanstack/react-query'

// Lib Imports
import { apiFetch } from '@/libs/api-client'

// Type Imports
import type { OrdersReportResponse, SalesReportResponse, VendorReportItemResponse } from '@/types/api/report'

export interface UseSalesReportParams {
  fromDate?: string
  toDate?: string
  enabled?: boolean
}

export const useSalesReport = ({ fromDate, toDate, enabled = true }: UseSalesReportParams = {}) =>
  useQuery({
    queryKey: ['reports', 'sales', { fromDate, toDate }],
    enabled,
    queryFn: () => {
      const query = new URLSearchParams()

      if (fromDate) query.set('fromDate', fromDate)
      if (toDate) query.set('toDate', toDate)

      const suffix = query.toString() ? `?${query}` : ''

      return apiFetch<SalesReportResponse>(`/api/reports/sales${suffix}`)
    }
  })

export const useOrdersReport = ({ enabled = true }: { enabled?: boolean } = {}) =>
  useQuery({
    queryKey: ['reports', 'orders'],
    enabled,
    queryFn: () => apiFetch<OrdersReportResponse>('/api/reports/orders')
  })

export const useVendorsReport = ({ top = 10, enabled = true }: { top?: number; enabled?: boolean } = {}) =>
  useQuery({
    queryKey: ['reports', 'vendors', top],
    enabled,
    queryFn: () => apiFetch<VendorReportItemResponse[]>(`/api/reports/vendors?top=${top}`)
  })
