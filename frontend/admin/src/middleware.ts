import { NextResponse } from 'next/server'
import type { NextRequest } from 'next/server'

const REFRESH_COOKIE = 'refresh_token'

// Public routes that never require a session.
const PUBLIC_PATHS = ['/login', '/register', '/forgot-password', '/error', '/under-maintenance']

// Gate on presence of the httpOnly refresh cookie only — this is a UX redirect, not the real
// security boundary. The backend re-validates the Bearer access token on every API call
// regardless; see ai/09-AI-Frontend-Skills.md §3 "Route protection".
export function middleware(request: NextRequest) {
  const { pathname } = request.nextUrl

  if (PUBLIC_PATHS.some(path => pathname.startsWith(path))) {
    return NextResponse.next()
  }

  const hasSession = request.cookies.has(REFRESH_COOKIE)

  if (!hasSession) {
    const loginUrl = new URL('/login', request.url)

    loginUrl.searchParams.set('redirectTo', pathname)

    return NextResponse.redirect(loginUrl)
  }

  return NextResponse.next()
}

export const config = {
  matcher: ['/((?!api|_next/static|_next/image|favicon.ico|images).*)']
}
