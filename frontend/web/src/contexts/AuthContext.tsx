'use client'

// React Imports
import { createContext, useCallback, useContext, useEffect, useState } from 'react'
import type { ReactNode } from 'react'

// Next Imports
import { useRouter } from 'next/navigation'

// Lib Imports
import { getAccessToken, setSession, clearSession } from '@/libs/auth-session'
import { toAuthUser } from '@/libs/jwt'
import type { AuthUser } from '@/libs/jwt'
import { ApiError, refreshAccessToken } from '@/libs/api-client'

interface LoginResponse {
  accessToken: string
  expiresAt: string
}

interface AuthContextValue {
  user: AuthUser | null
  isLoading: boolean
  login: (email: string, password: string) => Promise<void>
  logout: () => Promise<void>
}

const AuthContext = createContext<AuthContextValue | null>(null)

export const AuthProvider = ({ children }: { children: ReactNode }) => {
  const [user, setUser] = useState<AuthUser | null>(null)
  const [isLoading, setIsLoading] = useState(true)
  const router = useRouter()

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

    setSession(data.accessToken, data.expiresAt)
    setUser(toAuthUser(data.accessToken))
  }, [])

  const logout = useCallback(async () => {
    const accessToken = getAccessToken()

    await fetch('/api/auth/logout', {
      method: 'POST',
      headers: accessToken ? { Authorization: `Bearer ${accessToken}` } : undefined
    }).catch(() => null)

    clearSession()
    setUser(null)

    // Shared across the storefront and admin — send the user back to the login screen
    // for whichever surface they were on.
    const onAdmin = typeof window !== 'undefined' && window.location.pathname.startsWith('/admin')

    router.push(onAdmin ? '/admin/login' : '/login')
  }, [router])

  return <AuthContext.Provider value={{ user, isLoading, login, logout }}>{children}</AuthContext.Provider>
}

export const useAuth = (): AuthContextValue => {
  const context = useContext(AuthContext)

  if (!context) throw new Error('useAuth must be used within an AuthProvider')

  return context
}
