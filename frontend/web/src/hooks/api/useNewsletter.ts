// Third-party Imports
import { useQuery } from '@tanstack/react-query'

// Lib Imports
import { apiFetch } from '@/libs/api-client'

// Type Imports
import type { PagedResponse } from '@/types/api/common'
import type { NewsletterSubscriberResponse } from '@/types/api/newsletter'

export interface UseNewsletterSubscribersParams {
  page: number
  pageSize: number
  search?: string
  isActive?: boolean
}

export const useNewsletterSubscribers = (params: UseNewsletterSubscribersParams) =>
  useQuery({
    queryKey: ['newsletter-subscribers', params],
    queryFn: () => {
      const query = new URLSearchParams({ page: String(params.page), pageSize: String(params.pageSize) })

      if (params.search) query.set('search', params.search)
      if (params.isActive !== undefined) query.set('isActive', String(params.isActive))

      return apiFetch<PagedResponse<NewsletterSubscriberResponse>>(`/api/newsletter/subscribers?${query}`)
    }
  })
