// Third-party Imports
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'

// Lib Imports
import { apiFetch } from '@/libs/api-client'

// Type Imports
import type { WishlistItemResponse } from '@/types/api/wishlist'

export const useWishlist = (enabled = true) =>
  useQuery({
    queryKey: ['wishlist'],
    queryFn: () => apiFetch<WishlistItemResponse[]>('/api/wishlist'),
    enabled
  })

export const useAddToWishlist = () => {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (productId: string) =>
      apiFetch<WishlistItemResponse>('/api/wishlist', { method: 'POST', body: { productId } }),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['wishlist'] })
  })
}

export const useRemoveFromWishlist = () => {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (productId: string) => apiFetch<void>(`/api/wishlist/${productId}`, { method: 'DELETE' }),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['wishlist'] })
  })
}
