// Third-party Imports
import { useMutation } from '@tanstack/react-query'

// Lib Imports
import { apiFetch } from '@/libs/api-client'

// Type Imports
import type { ContactMessageRequest, ContactMessageResponse } from '@/types/api/contact'

export const useSubmitContact = () =>
  useMutation({
    mutationFn: (request: ContactMessageRequest) =>
      apiFetch<ContactMessageResponse>('/api/contact', { method: 'POST', body: request })
  })
