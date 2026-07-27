'use client'

// React Imports
import { createContext, useCallback, useContext, useEffect, useState } from 'react'
import type { ReactNode } from 'react'

// Next Imports
import { useRouter } from 'next/navigation'

// Third-party Imports
import { useQueryClient } from '@tanstack/react-query'

// Lib Imports
import { getAccessToken, setSession, clearSession } from '@/libs/auth-session'
import { toAuthUser } from '@/libs/jwt'
import type { AuthUser } from '@/libs/jwt'
import { ApiError, refreshAccessToken } from '@/libs/api-client'

interface LoginResponse {
  accessToken: string
  expiresAt: string
}

export interface RegisterData {
  email: string
  password: string
  firstName: string
  lastName: string
  phoneNumber?: string | null
}

interface AuthContextValue {
  user: AuthUser | null
  isLoading: boolean
  login: (email: string, password: string) => Promise<void>
  register: (data: RegisterData) => Promise<void>
  logout: () => Promise<void>
}

const AuthContext = createContext<AuthContextValue | null>(null)

export const AuthProvider = ({ children }: { children: ReactNode }) => {
  const [user, setUser] = useState<AuthUser | null>(null)
  const [isLoading, setIsLoading] = useState(true)
  const router = useRouter()
  const queryClient = useQueryClient()

  // Silent refresh on mount — recovers the session from the httpOnly refresh cookie after a
  // full page reload, since the access token itself lives only in memory (see §6 of the skills doc).
  // Uses the shared single-flight refresh from api-client: a concurrent data query's 401 retry
  // must join THIS refresh, not race a second one (token rotation flags reuse as theft).
  useEffect(() => {
    let cancelled = false

    const restoreSession = async () => {
      try {
        const refreshed = await refreshAccessToken()
        const accessToken = getAccessToken()

        if (!cancelled && refreshed && accessToken) {
          setUser(toAuthUser(accessToken))
        }
      } finally {
        if (!cancelled) setIsLoading(false)
      }
    }

    restoreSession()

    return () => {
      cancelled = true
    }
  }, [])

  const login = useCallback(async (email: string, password: string) => {
    const response = await fetch('/api/auth/login', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ email, password })
    })

    if (!response.ok) {
      const problem = await response.json().catch(() => null)

      throw new ApiError(response.status, problem?.title ?? 'Unauthorized', problem?.detail ?? 'Login failed.')
    }

    const data: LoginResponse = await response.json()

    // Also clear on the way in: a session can end without logout() running (expiry, another
    // tab, a hard reload), so signing in as someone else must not inherit their cache.
    queryClient.clear()
    setSession(data.accessToken, data.expiresAt)
    setUser(toAuthUser(data.accessToken))
  }, [queryClient])

  const register = useCallback(async (payload: RegisterData) => {
    const response = await fetch('/api/auth/register', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(payload)
    })

    if (!response.ok) {
      const problem = await response.json().catch(() => null)

      throw new ApiError(
        response.status,
        problem?.title ?? 'Registration failed',
        problem?.detail ?? 'Could not create your account.',
        problem?.errors
      )
    }

    const data: LoginResponse = await response.json()

    queryClient.clear()
    setSession(data.accessToken, data.expiresAt)
    setUser(toAuthUser(data.accessToken))
  }, [queryClient])

  const logout = useCallback(async () => {
    const accessToken = getAccessToken()

    await fetch('/api/auth/logout', {
      method: 'POST',
      headers: accessToken ? { Authorization: `Bearer ${accessToken}` } : undefined
    }).catch(() => null)

    clearSession()
    setUser(null)

    // Drop every cached query. Hooks gate on auth with `enabled`, but `enabled: false` only
    // stops refetching — the hook still serves whatever is already in the cache. Without this
    // the cart badge keeps its count after signing out, and more seriously the next person on
    // this browser can see the previous user's wishlist, orders and profile rendered from
    // cache before any request 401s. Clearing everything (rather than listing user-scoped
    // keys) means a newly added query can't be forgotten here later.
    queryClient.clear()

    // Shared across the storefront and admin — send the user back to the login screen
    // for whichever surface they were on.
    const onAdmin = typeof window !== 'undefined' && window.location.pathname.startsWith('/admin')

    router.push(onAdmin ? '/admin/login' : '/login')
  }, [router, queryClient])

  return <AuthContext.Provider value={{ user, isLoading, login, register, logout }}>{children}</AuthContext.Provider>
}

export const useAuth = (): AuthContextValue => {
  const context = useContext(AuthContext)

  if (!context) throw new Error('useAuth must be used within an AuthProvider')

  return context
}
