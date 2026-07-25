import { NextResponse } from 'next/server'
import type { NextRequest } from 'next/server'

const REFRESH_COOKIE = 'refresh_token'

interface AuthResponse {
  accessToken: string
  refreshToken: string
  expiresAt: string
}

// BFF proxy for customer self-registration — mirrors the login route: proxies to the backend,
// returns only the short-lived access token to the client, and stores the refresh token in an
// httpOnly cookie (never JS-accessible).
export async function POST(request: NextRequest) {
  const body = await request.json()

  const response = await fetch(`${process.env.NEXT_PUBLIC_API_URL}/api/auth/register`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(body)
  })

  if (!response.ok) {
    const problem = await response.json().catch(() => null)

    return NextResponse.json(problem ?? { title: 'Registration failed', status: response.status }, {
      status: response.status
    })
  }

  const auth: AuthResponse = await response.json()

  const nextResponse = NextResponse.json({ accessToken: auth.accessToken, expiresAt: auth.expiresAt })

  nextResponse.cookies.set(REFRESH_COOKIE, auth.refreshToken, {
    httpOnly: true,
    secure: process.env.NODE_ENV === 'production',
    sameSite: 'lax',
    path: '/',
    maxAge: 60 * 60 * 24 * 7
  })

  return nextResponse
}
