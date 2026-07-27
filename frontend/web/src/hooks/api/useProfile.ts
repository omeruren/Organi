// Third-party Imports
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'

// Lib Imports
import { apiFetch } from '@/libs/api-client'

// Type Imports
import type { ProfileResponse, UpdateProfileRequest } from '@/types/api/profile'

// `enabled` lets callers on anonymous-reachable pages (e.g. checkout) hold the fetch until auth
// resolves — GET /api/profile is authorized and would otherwise 401 for signed-out visitors.
export const useProfile = (enabled = true) =>
  useQuery({
    queryKey: ['profile'],
    queryFn: () => apiFetch<ProfileResponse>('/api/profile'),
    enabled
  })

export const useUpdateProfile = () => {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (request: UpdateProfileRequest) =>
      apiFetch<ProfileResponse>('/api/profile', { method: 'PUT', body: request }),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['profile'] })
  })
}
