// Third-party Imports
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'

// Lib Imports
import { apiFetch } from '@/libs/api-client'

// Type Imports
import type { PagedResponse } from '@/types/api/common'
import type { AdminReviewResponse } from '@/types/api/review'

export interface UseReviewsParams {
  page: number
  pageSize: number
  rating?: number
  productId?: string
}

export const useReviews = (params: UseReviewsParams) =>
  useQuery({
    queryKey: ['reviews', params],
    queryFn: () => {
      const query = new URLSearchParams({ page: String(params.page), pageSize: String(params.pageSize) })

      if (params.rating !== undefined) query.set('rating', String(params.rating))
      if (params.productId) query.set('productId', params.productId)

      return apiFetch<PagedResponse<AdminReviewResponse>>(`/api/reviews?${query}`)
    }
  })

export const useDeleteReview = () => {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (id: string) => apiFetch<void>(`/api/reviews/${id}`, { method: 'DELETE' }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['reviews'] })

      // Deleting a review recalculates the product's AverageRating/ReviewCount.
      queryClient.invalidateQueries({ queryKey: ['products'] })
    }
  })
}
