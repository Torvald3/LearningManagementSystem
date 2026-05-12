import { type FormEvent, useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { apiJson } from '../api/http'
import type { LoginRequest, LoginResponse } from '../api/types'
import { useAuth } from '../auth/AuthContext'

export function LoginPage() {
  const { setAccessToken } = useAuth()
  const navigate = useNavigate()
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState<string | null>(null)
  const [loading, setLoading] = useState(false)

  async function onSubmit(e: FormEvent) {
    e.preventDefault()
    setError(null)
    setLoading(true)
    try {
      const body: LoginRequest = { email, password }
      const res = await apiJson<LoginResponse>('/api/auth/login', {
        method: 'POST',
        body: JSON.stringify(body),
      })
      setAccessToken(res.accessToken)
      navigate('/courses')
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Помилка входу')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="page page-narrow">
      <h1>Вхід</h1>
      <form className="form" onSubmit={onSubmit}>
        <label className="field">
          <span>Email</span>
          <input
            type="email"
            autoComplete="username"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
          />
        </label>
        <label className="field">
          <span>Пароль</span>
          <input
            type="password"
            autoComplete="current-password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
          />
        </label>
        {error && <p className="error">{error}</p>}
        <button type="submit" className="btn btn-primary" disabled={loading}>
          {loading ? 'Зачекайте…' : 'Увійти'}
        </button>
      </form>
      <p className="muted">
        Немає акаунта? <Link to="/register">Реєстрація</Link>
      </p>
    </div>
  )
}
