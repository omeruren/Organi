// Server-only fetch for `generateMetadata` on public detail routes. Deliberately separate from
// `apiFetch` (which couples to the in-memory access token and the BFF refresh route, neither of
// which exists during server rendering). Anonymous GET of public endpoints only; never throws —
// returns null so metadata falls back to a sensible default instead of failing the render.
export async function serverFetch<T>(path: string): Promise<T | null> {
  const baseUrl = process.env.NEXT_PUBLIC_API_URL

  if (!baseUrl) return null

  try {
    const response = await fetch(`${baseUrl}${path}`, { headers: { Accept: 'application/json' } })

    if (!response.ok) return null

    return (await response.json()) as T
  } catch {
    return null
  }
}
