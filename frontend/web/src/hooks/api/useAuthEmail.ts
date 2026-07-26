// Third-party Imports
import { useMutation } from '@tanstack/react-query'

// Lib Imports
import { apiFetch } from '@/libs/api-client'

// These endpoints are anonymous and return no tokens or cookies, so they call the backend
// directly rather than going through a BFF route (unlike login/register/refresh/logout).

export const useForgotPassword = () =>
  useMutation({
    mutationFn: (email: string) => apiFetch<void>('/api/auth/forgot-password', { method: 'POST', body: { email } })
  })

export interface ResetPasswordRequest {
  email: string
  code: string
  newPassword: string
}

export const useResetPassword = () =>
  useMutation({
    mutationFn: (request: ResetPasswordRequest) =>
      apiFetch<void>('/api/auth/reset-password', { method: 'POST', body: request })
  })

export const useConfirmEmail = () =>
  useMutation({
    mutationFn: (token: string) => apiFetch<void>('/api/auth/confirm-email', { method: 'POST', body: { token } })
  })

export const useResendConfirmation = () =>
  useMutation({
    mutationFn: (email: string) => apiFetch<void>('/api/auth/resend-confirmation', { method: 'POST', body: { email } })
  })
