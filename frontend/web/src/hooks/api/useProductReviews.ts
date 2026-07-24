// Third-party Imports
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'

// Lib Imports
import { apiFetch } from '@/libs/api-client'

// Type Imports
import type { PagedResponse } from '@/types/api/common'
import type { ReviewResponse, CreateReviewRequest } from '@/types/api/review'

// Public per-product reviews (approved only) — GET /api/products/{productId}/reviews.
export const useProductReviews = (productId: string | null, page = 1, pageSize = 10) =>
  useQuery({
    queryKey: ['product-reviews', productId, page, pageSize],
    queryFn: () =>
      apiFetch<PagedResponse<ReviewResponse>>(`/api/products/${productId}/reviews?page=${page}&pageSize=${pageSize}`),
    enabled: !!productId
  })

// Create a review (auth + purchaser only — wired into the UI in B5 with store customer auth).
export const useCreateProductReview = (productId: string) => {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (request: CreateReviewRequest) =>
      apiFetch<ReviewResponse>(`/api/products/${productId}/reviews`, { method: 'POST', body: request }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['product-reviews', productId] })
      queryClient.invalidateQueries({ queryKey: ['products'] })
    }
  })
}
