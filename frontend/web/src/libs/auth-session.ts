// In-memory access token store. Deliberately NOT persisted to localStorage/sessionStorage —
// see ai/09-AI-Frontend-Skills.md §6. A page reload loses this; AuthProvider recovers it by
// calling /api/auth/refresh on mount, which reads the httpOnly refresh-token cookie.

let accessToken: string | null = null
let expiresAt: number | null = null

export function getAccessToken(): string | null {
  return accessToken
}

export function getTokenExpiresAt(): number | null {
  return expiresAt
}

export function setSession(token: string | null, expiresAtIso?: string | null): void {
  accessToken = token
  expiresAt = expiresAtIso ? new Date(expiresAtIso).getTime() : null
}

export function clearSession(): void {
  accessToken = null
  expiresAt = null
}
