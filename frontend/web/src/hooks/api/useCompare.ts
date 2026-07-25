// Third-party Imports
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'

// Lib Imports
import { apiFetch } from '@/libs/api-client'

// Type Imports
import type { CompareItemResponse } from '@/types/api/compare'

export const useCompare = (enabled = true) =>
  useQuery({
    queryKey: ['compare'],
    queryFn: () => apiFetch<CompareItemResponse[]>('/api/compare'),
    enabled
  })

export const useAddToCompare = () => {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (productId: string) =>
      apiFetch<CompareItemResponse>('/api/compare', { method: 'POST', body: { productId } }),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['compare'] })
  })
}

export const useRemoveFromCompare = () => {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (productId: string) => apiFetch<void>(`/api/compare/${productId}`, { method: 'DELETE' }),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['compare'] })
  })
}
