import { NextResponse } from 'next/server'
import type { NextRequest } from 'next/server'

const REFRESH_COOKIE = 'refresh_token'

// Admin auth pages that must stay reachable without a session.
const ADMIN_PUBLIC_PATHS = ['/admin/login', '/admin/register', '/admin/forgot-password', '/admin/error']

// Customer storefront paths that require a signed-in session (the rest of the store is public).
const CUSTOMER_PROTECTED_PREFIXES = ['/account', '/checkout']

// Gate on presence of the httpOnly refresh cookie only — this is a UX redirect, not the real
// security boundary. The backend re-validates the Bearer access token on every API call
// regardless; see ai/09-AI-Frontend-Skills.md §3 "Route protection".
export function middleware(request: NextRequest) {
  const { pathname } = request.nextUrl
  const hasSession = request.cookies.has(REFRESH_COOKIE)

  // Admin surface: everything under /admin except its own auth pages.
  const isAdmin = pathname === '/admin' || pathname.startsWith('/admin/')
  const isAdminPublic = ADMIN_PUBLIC_PATHS.some(path => pathname.startsWith(path))

  if (isAdmin && !isAdminPublic && !hasSession) {
    const loginUrl = new URL('/admin/login', request.url)

    loginUrl.searchParams.set('redirectTo', pathname)

    return NextResponse.redirect(loginUrl)
  }

  // Customer surface: only account + checkout need a session; the storefront is otherwise public.
  const isCustomerProtected = CUSTOMER_PROTECTED_PREFIXES.some(
    prefix => pathname === prefix || pathname.startsWith(`${prefix}/`)
  )

  if (isCustomerProtected && !hasSession) {
    const loginUrl = new URL('/login', request.url)

    loginUrl.searchParams.set('redirectTo', pathname)

    return NextResponse.redirect(loginUrl)
  }

  return NextResponse.next()
}

export const config = {
  matcher: ['/((?!api|_next/static|_next/image|favicon.ico|images).*)']
}
