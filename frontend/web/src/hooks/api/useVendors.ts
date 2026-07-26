// Third-party Imports
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'

// Lib Imports
import { apiFetch } from '@/libs/api-client'

// Type Imports
import type { PagedResponse } from '@/types/api/common'
import type { ProductSummaryResponse } from '@/types/api/product'
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

// Storefront vendor store page — the customer addresses a vendor by slug.
export const useVendorBySlug = (slug: string | null) =>
  useQuery({
    queryKey: ['vendors', 'slug', slug],
    queryFn: () => apiFetch<VendorResponse>(`/api/vendors/slug/${slug}`),
    enabled: !!slug
  })

export interface UseVendorProductsParams {
  vendorId: string | null
  page: number
  pageSize: number
  sortBy?: string
  sortOrder?: string
  enabled?: boolean
}

// A single vendor's products — GET /api/vendors/{id}/products (public, approved-vendor products).
export const useVendorProducts = ({ enabled = true, ...params }: UseVendorProductsParams) =>
  useQuery({
    queryKey: ['vendors', 'products', params],
    enabled: enabled && !!params.vendorId,
    queryFn: () => {
      const query = new URLSearchParams({ page: String(params.page), pageSize: String(params.pageSize) })

      if (params.sortBy) query.set('sortBy', params.sortBy)
      if (params.sortOrder) query.set('sortOrder', params.sortOrder)

      return apiFetch<PagedResponse<ProductSummaryResponse>>(`/api/vendors/${params.vendorId}/products?${query}`)
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
