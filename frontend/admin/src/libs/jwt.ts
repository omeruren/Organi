// Mirrors the exact claims JwtTokenService.cs (Organi.Server.Infrastructure/Security) writes into
// the access token. Two claim types are deliberately NOT simple "role"/"roles" keys — .NET's
// JwtSecurityTokenHandler serializes ASP.NET's ClaimTypes.Role as its full default URI, and
// collapses repeated claims of the same type into a single string when there's exactly one value
// but a JSON array when there are multiple. Both shapes must be handled below.
const ROLE_CLAIM = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'

interface RawAccessTokenPayload {
  sub: string
  email: string
  name: string
  permissions?: string | string[]
  vendor_id?: string
  exp: number
  iss: string
  aud: string
  [ROLE_CLAIM]?: string | string[]
}

export interface AuthUser {
  id: string
  email: string
  name: string
  roles: string[]
  permissions: string[]
  vendorId: string | null
  expiresAt: number
}

const toArray = (value: string | string[] | undefined): string[] => {
  if (!value) return []

  return Array.isArray(value) ? value : [value]
}

// Client-side decode only — no signature verification. The backend already verified the
// token; this is purely for reading claims to drive UI (nav visibility, role gates).
function decodeAccessToken(token: string): RawAccessTokenPayload | null {
  try {
    const payload = token.split('.')[1]

    if (!payload) return null

    const json = atob(payload.replace(/-/g, '+').replace(/_/g, '/'))

    return JSON.parse(json) as RawAccessTokenPayload
  } catch {
    return null
  }
}

export function toAuthUser(token: string): AuthUser | null {
  const decoded = decodeAccessToken(token)

  if (!decoded) return null

  return {
    id: decoded.sub,
    email: decoded.email,
    name: decoded.name,
    roles: toArray(decoded[ROLE_CLAIM]),
    permissions: toArray(decoded.permissions),
    vendorId: decoded.vendor_id ?? null,
    expiresAt: decoded.exp
  }
}
