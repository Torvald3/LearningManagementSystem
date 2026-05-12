import { type FormEvent, useState } from 'react'
import { Link } from 'react-router-dom'
import { apiJson } from '../api/http'
import type { ConfirmEmailRequest } from '../api/types'

export function ConfirmEmailPage() {
  const [userId, setUserId] = useState('')
  const [token, setToken] = useState('')
  const [message, setMessage] = useState<string | null>(null)
  const [error, setError] = useState<string | null>(null)
  const [loading, setLoading] = useState(false)

  async function onSubmit(e: FormEvent) {
    e.preventDefault()
    setError(null)
    setMessage(null)
    setLoading(true)
    try {
      const body: ConfirmEmailRequest = { userId, token }
      await apiJson<unknown>('/api/auth/confirm-email', {
        method: 'POST',
        body: JSON.stringify(body),
      })
      setMessage('Email підтверджено. Тепер можна увійти.')
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Помилка')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="page page-narrow">
      <h1>Підтвердження email</h1>
      <form className="form" onSubmit={onSubmit}>
        <label className="field">
          <span>UserId (GUID)</span>
          <input value={userId} onChange={(e) => setUserId(e.target.value)} required />
        </label>
        <label className="field">
          <span>Токен</span>
          <textarea value={token} onChange={(e) => setToken(e.target.value)} required rows={3} />
        </label>
        {error && <p className="error">{error}</p>}
        {message && <p className="success">{message}</p>}
        <button type="submit" className="btn btn-primary" disabled={loading}>
          {loading ? 'Зачекайте…' : 'Підтвердити'}
        </button>
      </form>
      <p className="muted">
        <Link to="/login">Назад до входу</Link>
      </p>
    </div>
  )
}
