// Third-party Imports
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'

// Lib Imports
import { apiFetch } from '@/libs/api-client'

// Type Imports
import type { CartResponse } from '@/types/api/cart'

// The cart is per-user (auth required) — callers gate the query with `enabled` on auth state.
export const useCart = (enabled = true) =>
  useQuery({
    queryKey: ['cart'],
    queryFn: () => apiFetch<CartResponse>('/api/cart'),
    enabled
  })

export const useAddToCart = () => {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({ productId, quantity }: { productId: string; quantity: number }) =>
      apiFetch<CartResponse>('/api/cart/items', { method: 'POST', body: { productId, quantity } }),
    onSuccess: data => queryClient.setQueryData(['cart'], data)
  })
}

export const useUpdateCartItem = () => {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({ id, quantity }: { id: string; quantity: number }) =>
      apiFetch<CartResponse>(`/api/cart/items/${id}`, { method: 'PUT', body: { quantity } }),
    onSuccess: data => queryClient.setQueryData(['cart'], data)
  })
}

export const useRemoveCartItem = () => {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (id: string) => apiFetch<void>(`/api/cart/items/${id}`, { method: 'DELETE' }),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['cart'] })
  })
}

export const useClearCart = () => {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: () => apiFetch<void>('/api/cart', { method: 'DELETE' }),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['cart'] })
  })
}
