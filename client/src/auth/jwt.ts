/** Reads `sub` from JWT payload (client-side only, for UI defaults). */
export function readJwtSubject(accessToken: string): string | null {
  try {
    const parts = accessToken.split('.')
    if (parts.length < 2) return null
    const payload = parts[1]
    const json = JSON.parse(atob(payload.replace(/-/g, '+').replace(/_/g, '/'))) as {
      sub?: string
    }
    return json.sub ?? null
  } catch {
    return null
  }
}
