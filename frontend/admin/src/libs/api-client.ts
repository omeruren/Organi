import { getAccessToken, setSession, clearSession } from '@/libs/auth-session'
import type { ProblemDetails, ValidationProblemDetails } from '@/types/api/common'

export class ApiError extends Error {
  status: number
  title: string
  errors?: Record<string, string[]>

  constructor(status: number, title: string, detail: string, errors?: Record<string, string[]>) {
    super(detail)
    this.status = status
    this.title = title
    this.errors = errors
  }
}

type RequestOptions = Omit<RequestInit, 'body'> & { body?: unknown }

let refreshPromise: Promise<boolean> | null = null

// Calls the BFF refresh route (reads the httpOnly cookie server-side), updates the in-memory
// access token on success. Concurrent 401s share a single in-flight refresh call.
async function refreshAccessToken(): Promise<boolean> {
  if (!refreshPromise) {
    refreshPromise = (async () => {
      try {
        const response = await fetch('/api/auth/refresh', { method: 'POST' })

        if (!response.ok) {
          clearSession()

          return false
        }

        const data = (await response.json()) as { accessToken: string; expiresAt: string }

        setSession(data.accessToken, data.expiresAt)

        return true
      } catch {
        clearSession()

        return false
      } finally {
        refreshPromise = null
      }
    })()
  }

  return refreshPromise
}

async function parseProblem(response: Response): Promise<ProblemDetails | ValidationProblemDetails | null> {
  try {
    return await response.json()
  } catch {
    return null
  }
}

async function rawFetch(path: string, options: RequestOptions): Promise<Response> {
  const accessToken = getAccessToken()

  return fetch(`${process.env.NEXT_PUBLIC_API_URL}${path}`, {
    ...options,
    headers: {
      'Content-Type': 'application/json',
      ...(accessToken && { Authorization: `Bearer ${accessToken}` }),
      ...options.headers
    },
    body: options.body !== undefined ? JSON.stringify(options.body) : undefined
  })
}

export async function apiFetch<T>(path: string, options: RequestOptions = {}, isRetry = false): Promise<T> {
  const response = await rawFetch(path, options)

  if (response.status === 401 && !isRetry) {
    const refreshed = await refreshAccessToken()

    if (refreshed) {
      return apiFetch<T>(path, options, true)
    }
  }

  if (!response.ok) {
    const problem = await parseProblem(response)

    throw new ApiError(
      response.status,
      problem?.title ?? 'Request failed',
      problem?.detail ?? 'An unexpected error occurred.',
      (problem as ValidationProblemDetails | null)?.errors
    )
  }

  if (response.status === 204) {
    return undefined as T
  }

  return response.json() as Promise<T>
}
