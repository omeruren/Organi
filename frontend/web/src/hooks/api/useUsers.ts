// Third-party Imports
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'

// Lib Imports
import { apiFetch } from '@/libs/api-client'

// Type Imports
import type { PagedResponse } from '@/types/api/common'
import type { UserResponse } from '@/types/api/user'

export interface UseUsersParams {
  page: number
  pageSize: number
  search?: string
  isActive?: boolean
}

export const useUsers = (params: UseUsersParams) =>
  useQuery({
    queryKey: ['users', params],
    queryFn: () => {
      const query = new URLSearchParams({ page: String(params.page), pageSize: String(params.pageSize) })

      if (params.search) query.set('search', params.search)
      if (params.isActive !== undefined) query.set('isActive', String(params.isActive))

      return apiFetch<PagedResponse<UserResponse>>(`/api/users?${query}`)
    }
  })

export const useActivateUser = () => {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (id: string) => apiFetch<UserResponse>(`/api/users/${id}/activate`, { method: 'PUT' }),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['users'] })
  })
}

export const useDeactivateUser = () => {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (id: string) => apiFetch<UserResponse>(`/api/users/${id}/deactivate`, { method: 'PUT' }),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['users'] })
  })
}

export const useAssignUserRoles = () => {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({ id, roleNames }: { id: string; roleNames: string[] }) =>
      apiFetch<UserResponse>(`/api/users/${id}/roles`, { method: 'PUT', body: { roleNames } }),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['users'] })
  })
}
