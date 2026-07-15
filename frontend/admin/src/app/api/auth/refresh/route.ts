import { cookies } from 'next/headers'
import { NextResponse } from 'next/server'

const REFRESH_COOKIE = 'refresh_token'

interface AuthResponse {
  accessToken: string
  refreshToken: string
  expiresAt: string
}

// Reads the httpOnly refresh cookie (never exposed to client JS), rotates it per the backend's
// refresh-token-rotation policy, and returns a fresh access token. See §6 of the frontend skills doc.
export async function POST() {
  const cookieStore = cookies()
  const refreshToken = cookieStore.get(REFRESH_COOKIE)?.value

  if (!refreshToken) {
    return NextResponse.json({ title: 'Unauthorized', status: 401, detail: 'No session.' }, { status: 401 })
  }

  const response = await fetch(`${process.env.NEXT_PUBLIC_API_URL}/api/auth/refresh`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ refreshToken })
  })

  if (!response.ok) {
    const problem = await response.json().catch(() => null)

    const nextResponse = NextResponse.json(problem ?? { title: 'Unauthorized', status: response.status }, {
      status: response.status
    })

    nextResponse.cookies.delete(REFRESH_COOKIE)

    return nextResponse
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
