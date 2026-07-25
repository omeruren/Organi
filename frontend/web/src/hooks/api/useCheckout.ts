// Third-party Imports
import { useMutation, useQueryClient } from '@tanstack/react-query'

// Lib Imports
import { apiFetch } from '@/libs/api-client'

// Type Imports
import type { CreateOrderRequest, OrderResponse } from '@/types/api/order'

// Checkout — POST /api/orders builds the order from the current user's cart and clears it.
export const useCreateOrder = () => {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (request: CreateOrderRequest) =>
      apiFetch<OrderResponse>('/api/orders', { method: 'POST', body: request }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['cart'] })
      queryClient.invalidateQueries({ queryKey: ['orders'] })
    }
  })
}
