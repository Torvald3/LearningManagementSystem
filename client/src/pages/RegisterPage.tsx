import { type FormEvent, useState } from 'react'
import { Link } from 'react-router-dom'
import { apiJson } from '../api/http'
import type { RegisterUserRequest, RegisterUserResponse } from '../api/types'

export function RegisterPage() {
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [username, setUsername] = useState('')
  const [error, setError] = useState<string | null>(null)
  const [loading, setLoading] = useState(false)
  const [done, setDone] = useState<RegisterUserResponse | null>(null)

  async function onSubmit(e: FormEvent) {
    e.preventDefault()
    setError(null)
    setLoading(true)
    try {
      const body: RegisterUserRequest = { email, password, username }
      const res = await apiJson<RegisterUserResponse>('/api/auth/register', {
        method: 'POST',
        body: JSON.stringify(body),
      })
      setDone(res)
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Помилка реєстрації')
    } finally {
      setLoading(false)
    }
  }

  if (done) {
    return (
      <div className="page page-narrow">
        <h1>Акаунт створено</h1>
        <p>
          Якщо увімкнено підтвердження email, використайте токен нижче на сторінці підтвердження.
        </p>
        <dl className="kv">
          <dt>UserId</dt>
          <dd>
            <code>{done.userId}</code>
          </dd>
          <dt>Email</dt>
          <dd>{done.email}</dd>
          {done.confirmationToken != null && (
            <>
              <dt>Токен підтвердження</dt>
              <dd>
                <code className="break-all">{done.confirmationToken}</code>
              </dd>
            </>
          )}
        </dl>
        <Link to="/confirm-email" className="btn btn-primary">
          Підтвердити email
        </Link>
        <p className="muted">
          Потім можна <Link to="/login">увійти</Link>.
        </p>
      </div>
    )
  }

  return (
    <div className="page page-narrow">
      <h1>Реєстрація</h1>
      <form className="form" onSubmit={onSubmit}>
        <label className="field">
          <span>Ім’я користувача</span>
          <input
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            required
            minLength={2}
          />
        </label>
        <label className="field">
          <span>Email</span>
          <input
            type="email"
            autoComplete="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
          />
        </label>
        <label className="field">
          <span>Пароль</span>
          <input
            type="password"
            autoComplete="new-password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
            minLength={6}
          />
        </label>
        {error && <p className="error">{error}</p>}
        <button type="submit" className="btn btn-primary" disabled={loading}>
          {loading ? 'Зачекайте…' : 'Зареєструватися'}
        </button>
      </form>
      <p className="muted">
        Вже є акаунт? <Link to="/login">Увійти</Link>
      </p>
    </div>
  )
}
