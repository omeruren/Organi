// Third-party Imports
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'

// Lib Imports
import { apiFetch } from '@/libs/api-client'

// Type Imports
import type { PagedResponse } from '@/types/api/common'
import type { VendorResponse } from '@/types/api/vendor'

export interface UseVendorsParams {
  page: number
  pageSize: number
  status?: string
  search?: string
}

export const useVendors = (params: UseVendorsParams) =>
  useQuery({
    queryKey: ['vendors', params],
    queryFn: () => {
      const query = new URLSearchParams({ page: String(params.page), pageSize: String(params.pageSize) })

      if (params.status) query.set('status', params.status)
      if (params.search) query.set('search', params.search)

      return apiFetch<PagedResponse<VendorResponse>>(`/api/vendors?${query}`)
    }
  })

export const useApproveVendor = () => {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (id: string) => apiFetch<VendorResponse>(`/api/vendors/${id}/approve`, { method: 'POST' }),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['vendors'] })
  })
}

export const useSuspendVendor = () => {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (id: string) => apiFetch<VendorResponse>(`/api/vendors/${id}/suspend`, { method: 'POST' }),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['vendors'] })
  })
}
