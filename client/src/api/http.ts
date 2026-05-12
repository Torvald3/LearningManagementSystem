import { apiBaseUrl } from '../config'

export class HttpError extends Error {
  readonly status: number
  readonly body: unknown

  constructor(status: number, body: unknown) {
    super(`HTTP ${status}`)
    this.name = 'HttpError'
    this.status = status
    this.body = body
  }
}

function formatBody(detail: unknown): string {
  if (detail == null) return ''
  if (typeof detail === 'string') return detail
  try {
    return JSON.stringify(detail)
  } catch {
    return String(detail)
  }
}

export async function apiJson<T>(
  path: string,
  init: RequestInit & { accessToken?: string | null } = {},
): Promise<T> {
  const { accessToken, headers: initHeaders, body, ...rest } = init
  const headers = new Headers(initHeaders)
  if (body != null && typeof body === 'string' && !headers.has('Content-Type')) {
    headers.set('Content-Type', 'application/json')
  }
  if (accessToken) {
    headers.set('Authorization', `Bearer ${accessToken}`)
  }

  const res = await fetch(`${apiBaseUrl}${path}`, {
    ...rest,
    body,
    headers,
  })

  if (!res.ok) {
    let detail: unknown
    const text = await res.text()
    try {
      detail = text ? JSON.parse(text) : text
    } catch {
      detail = text
    }
    const err = new HttpError(res.status, detail)
    err.message = formatBody(detail) || err.message
    throw err
  }

  if (res.status === 204) {
    return undefined as T
  }

  const ct = res.headers.get('content-type')
  if (!ct?.includes('application/json')) {
    return (await res.text()) as T
  }

  return res.json() as Promise<T>
}
