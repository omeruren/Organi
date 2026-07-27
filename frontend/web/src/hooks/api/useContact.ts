// Third-party Imports
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'

// Lib Imports
import { apiFetch } from '@/libs/api-client'

// Type Imports
import type { PagedResponse } from '@/types/api/common'
import type { ContactMessageRequest, ContactMessageResponse } from '@/types/api/contact'

export const useSubmitContact = () =>
  useMutation({
    mutationFn: (request: ContactMessageRequest) =>
      apiFetch<ContactMessageResponse>('/api/contact', { method: 'POST', body: request })
  })

export interface UseContactMessagesParams {
  page: number
  pageSize: number
  search?: string
  isHandled?: boolean
}

// Admin triage list — GET /api/contact is IsAdmin-only.
export const useContactMessages = (params: UseContactMessagesParams) =>
  useQuery({
    queryKey: ['contact-messages', params],
    queryFn: () => {
      const query = new URLSearchParams({ page: String(params.page), pageSize: String(params.pageSize) })

      if (params.search) query.set('search', params.search)
      if (params.isHandled !== undefined) query.set('isHandled', String(params.isHandled))

      return apiFetch<PagedResponse<ContactMessageResponse>>(`/api/contact?${query}`)
    }
  })

export const useSetContactMessageHandled = () => {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({ id, isHandled }: { id: string; isHandled: boolean }) =>
      apiFetch<ContactMessageResponse>(`/api/contact/${id}/handled`, { method: 'PATCH', body: { isHandled } }),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['contact-messages'] })
  })
}
