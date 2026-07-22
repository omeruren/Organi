// Third-party Imports
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'

// Lib Imports
import { apiFetch } from '@/libs/api-client'

// Type Imports
import type { ProfileResponse, UpdateProfileRequest } from '@/types/api/profile'

export const useProfile = () =>
  useQuery({
    queryKey: ['profile'],
    queryFn: () => apiFetch<ProfileResponse>('/api/profile')
  })

export const useUpdateProfile = () => {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (request: UpdateProfileRequest) =>
      apiFetch<ProfileResponse>('/api/profile', { method: 'PUT', body: request }),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['profile'] })
  })
}
