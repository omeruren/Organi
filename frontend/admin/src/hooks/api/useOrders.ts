// Third-party Imports
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'

// Lib Imports
import { apiFetch } from '@/libs/api-client'

// Type Imports
import type { PagedResponse } from '@/types/api/common'
import type { OrderResponse, OrderSummaryResponse } from '@/types/api/order'

export interface UseOrdersParams {
  page: number
  pageSize: number
  status?: string
  search?: string

  // 'admin' hits GET /api/orders (IsAdmin); 'vendor' hits the vendor-scoped
  // GET /api/vendors/dashboard/orders (orders containing the vendor's items).
  scope: 'admin' | 'vendor'
}

export const useOrders = (params: UseOrdersParams) =>
  useQuery({
    queryKey: ['orders', params],
    queryFn: () => {
      const query = new URLSearchParams({ page: String(params.page), pageSize: String(params.pageSize) })

      if (params.status) query.set('status', params.status)
      if (params.search) query.set('search', params.search)

      const path = params.scope === 'vendor' ? '/api/vendors/dashboard/orders' : '/api/orders'

      return apiFetch<PagedResponse<OrderSummaryResponse>>(`${path}?${query}`)
    }
  })

export const useOrder = (id: string | null) =>
  useQuery({
    queryKey: ['orders', 'detail', id],
    queryFn: () => apiFetch<OrderResponse>(`/api/orders/${id}`),
    enabled: id !== null
  })

export const useConfirmOrder = () => {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (id: string) => apiFetch<OrderResponse>(`/api/orders/${id}/confirm`, { method: 'POST' }),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['orders'] })
  })
}

export const useShipOrder = () => {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (id: string) => apiFetch<OrderResponse>(`/api/orders/${id}/ship`, { method: 'POST' }),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['orders'] })
  })
}

export const useDeliverOrder = () => {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (id: string) => apiFetch<OrderResponse>(`/api/orders/${id}/deliver`, { method: 'POST' }),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['orders'] })
  })
}

export const useCancelOrder = () => {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({ id, reason }: { id: string; reason: string | null }) =>
      apiFetch<OrderResponse>(`/api/orders/${id}/cancel`, { method: 'POST', body: { reason } }),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['orders'] })
  })
}
