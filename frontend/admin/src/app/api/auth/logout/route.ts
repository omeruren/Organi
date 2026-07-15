import { NextResponse } from 'next/server'
import type { NextRequest } from 'next/server'

const REFRESH_COOKIE = 'refresh_token'

// Forwards the caller's access token to the backend's logout endpoint (revokes all refresh
// tokens server-side), then always clears the local session cookie regardless of the backend
// call's outcome — an already-expired/invalid access token must not block local logout.
export async function POST(request: NextRequest) {
  const authorization = request.headers.get('authorization')

  if (authorization) {
    await fetch(`${process.env.NEXT_PUBLIC_API_URL}/api/auth/logout`, {
      method: 'POST',
      headers: { Authorization: authorization }
    }).catch(() => null)
  }

  const nextResponse = NextResponse.json({ success: true })

  nextResponse.cookies.delete(REFRESH_COOKIE)

  return nextResponse
}
