// Third-party Imports
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'

// Lib Imports
import { apiFetch } from '@/libs/api-client'

// Type Imports
import type { PagedResponse } from '@/types/api/common'
import type { CouponResponse, CreateCouponRequest, UpdateCouponRequest } from '@/types/api/coupon'

interface UseCouponsParams {
  page: number
  pageSize: number
}

export const useCoupons = (params: UseCouponsParams) =>
  useQuery({
    queryKey: ['coupons', params],
    queryFn: () =>
      apiFetch<PagedResponse<CouponResponse>>(`/api/coupons?page=${params.page}&pageSize=${params.pageSize}`)
  })

export const useCreateCoupon = () => {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (request: CreateCouponRequest) =>
      apiFetch<CouponResponse>('/api/coupons', { method: 'POST', body: request }),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['coupons'] })
  })
}

export const useUpdateCoupon = () => {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({ id, request }: { id: string; request: UpdateCouponRequest }) =>
      apiFetch<CouponResponse>(`/api/coupons/${id}`, { method: 'PUT', body: request }),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['coupons'] })
  })
}

export const useDeleteCoupon = () => {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (id: string) => apiFetch<void>(`/api/coupons/${id}`, { method: 'DELETE' }),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['coupons'] })
  })
}
