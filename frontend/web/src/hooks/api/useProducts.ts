// Third-party Imports
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'

// Lib Imports
import { apiFetch } from '@/libs/api-client'

// Type Imports
import type { PagedResponse } from '@/types/api/common'
import type { ProductResponse, ProductSummaryResponse, CreateProductRequest, UpdateProductRequest } from '@/types/api/product'

export interface UseProductsParams {
  page: number
  pageSize: number
  search?: string
  categoryId?: string
  status?: string
  isOrganic?: boolean
  vendorId?: string
  minPrice?: number
  maxPrice?: number
  sortBy?: string
  sortOrder?: string
  enabled?: boolean
}

export const useProducts = ({ enabled = true, ...params }: UseProductsParams) =>
  useQuery({
    queryKey: ['products', params],
    enabled,
    queryFn: () => {
      const query = new URLSearchParams({ page: String(params.page), pageSize: String(params.pageSize) })

      if (params.search) query.set('search', params.search)
      if (params.categoryId) query.set('categoryId', params.categoryId)
      if (params.status) query.set('status', params.status)
      if (params.isOrganic !== undefined) query.set('isOrganic', String(params.isOrganic))
      if (params.vendorId) query.set('vendorId', params.vendorId)
      if (params.minPrice !== undefined) query.set('minPrice', String(params.minPrice))
      if (params.maxPrice !== undefined) query.set('maxPrice', String(params.maxPrice))
      if (params.sortBy) query.set('sortBy', params.sortBy)
      if (params.sortOrder) query.set('sortOrder', params.sortOrder)

      return apiFetch<PagedResponse<ProductSummaryResponse>>(`/api/products?${query}`)
    }
  })

// Full detail — required before editing, since list rows lack sku/stock/images and a PUT
// seeded from a summary row would wipe the product's images (replace-all semantics).
export const useProduct = (id: string | null) =>
  useQuery({
    queryKey: ['products', 'detail', id],
    queryFn: () => apiFetch<ProductResponse>(`/api/products/${id}`),
    enabled: id !== null
  })

// Storefront product detail — the customer PDP addresses products by slug.
export const useProductBySlug = (slug: string | null) =>
  useQuery({
    queryKey: ['products', 'slug', slug],
    queryFn: () => apiFetch<ProductResponse>(`/api/products/slug/${slug}`),
    enabled: !!slug
  })

export const useCreateProduct = () => {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (request: CreateProductRequest) =>
      apiFetch<ProductResponse>('/api/products', { method: 'POST', body: request }),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['products'] })
  })
}

export const useUpdateProduct = () => {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({ id, request }: { id: string; request: UpdateProductRequest }) =>
      apiFetch<ProductResponse>(`/api/products/${id}`, { method: 'PUT', body: request }),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['products'] })
  })
}

export const useDeleteProduct = () => {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (id: string) => apiFetch<void>(`/api/products/${id}`, { method: 'DELETE' }),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['products'] })
  })
}
