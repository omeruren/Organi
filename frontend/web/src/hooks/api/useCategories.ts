// Third-party Imports
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'

// Lib Imports
import { apiFetch } from '@/libs/api-client'

// Type Imports
import type { CategoryResponse, CreateCategoryRequest, UpdateCategoryRequest } from '@/types/api/category'

// Plain array (nested tree), not paginated — see CategoryEndpoints.cs
export const useCategories = () =>
  useQuery({
    queryKey: ['categories'],
    queryFn: () => apiFetch<CategoryResponse[]>('/api/categories')
  })

export const useCreateCategory = () => {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (request: CreateCategoryRequest) =>
      apiFetch<CategoryResponse>('/api/categories', { method: 'POST', body: request }),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['categories'] })
  })
}

export const useUpdateCategory = () => {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({ id, request }: { id: string; request: UpdateCategoryRequest }) =>
      apiFetch<CategoryResponse>(`/api/categories/${id}`, { method: 'PUT', body: request }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['categories'] })

      // Product rows denormalize categoryName — a rename would leave them stale.
      queryClient.invalidateQueries({ queryKey: ['products'] })
    }
  })
}

export const useDeleteCategory = () => {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (id: string) => apiFetch<void>(`/api/categories/${id}`, { method: 'DELETE' }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['categories'] })
      queryClient.invalidateQueries({ queryKey: ['products'] })
    }
  })
}
