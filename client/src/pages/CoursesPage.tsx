import { useEffect, useState, type FormEvent } from 'react'
import { Link } from 'react-router-dom'
import { apiJson } from '../api/http'
import type { CourseResponse, CreateCourseRequest } from '../api/types'
import { useAuth } from '../auth/AuthContext'

export function CoursesPage() {
  const { accessToken, userId } = useAuth()
  const [courses, setCourses] = useState<CourseResponse[] | null>(null)
  const [error, setError] = useState<string | null>(null)
  const [title, setTitle] = useState('')
  const [theme, setTheme] = useState('')
  const [description, setDescription] = useState('')
  const [creating, setCreating] = useState(false)

  useEffect(() => {
    let cancelled = false
    ;(async () => {
      try {
        const list = await apiJson<CourseResponse[]>('/api/courses')
        if (!cancelled) setCourses(list)
      } catch (e: unknown) {
        if (!cancelled) setError(e instanceof Error ? e.message : 'Не вдалося завантажити курси')
      }
    })()
    return () => {
      cancelled = true
    }
  }, [])

  async function createCourse(e: FormEvent) {
    e.preventDefault()
    if (!userId) return
    setError(null)
    setCreating(true)
    try {
      const body: CreateCourseRequest = {
        authorId: userId,
        title,
        theme,
        description,
      }
      const created = await apiJson<CourseResponse>('/api/courses', {
        method: 'POST',
        body: JSON.stringify(body),
        accessToken,
      })
      setCourses((prev) => (prev ? [created, ...prev] : [created]))
      setTitle('')
      setTheme('')
      setDescription('')
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Помилка створення')
    } finally {
      setCreating(false)
    }
  }

  return (
    <div className="page">
      <h1>Курси</h1>
      {error && <p className="error">{error}</p>}
      {courses === null && <p>Завантаження…</p>}
      {courses && courses.length === 0 && <p className="muted">Поки що немає курсів.</p>}
      {courses && courses.length > 0 && (
        <ul className="course-list">
          {courses.map((c) => (
            <li key={c.id}>
              <Link to={`/courses/${c.id}`} className="course-link">
                <strong>{c.title}</strong>
                <span className="muted">{c.theme}</span>
              </Link>
            </li>
          ))}
        </ul>
      )}

      {accessToken && userId && (
        <section className="panel">
          <h2>Створити курс</h2>
          <p className="muted small">
            Автор визначається з вашого токена: <code>{userId}</code>
          </p>
          <form className="form form-inline" onSubmit={createCourse}>
            <label className="field">
              <span>Назва</span>
              <input value={title} onChange={(e) => setTitle(e.target.value)} required />
            </label>
            <label className="field">
              <span>Тема</span>
              <input value={theme} onChange={(e) => setTheme(e.target.value)} required />
            </label>
            <label className="field field-grow">
              <span>Опис</span>
              <textarea
                value={description}
                onChange={(e) => setDescription(e.target.value)}
                required
                rows={2}
              />
            </label>
            <button type="submit" className="btn btn-primary" disabled={creating}>
              {creating ? '…' : 'Створити'}
            </button>
          </form>
        </section>
      )}

      {!accessToken && (
        <p className="muted">
          Щоб створювати курси через UI, <Link to="/login">увійдіть</Link> (якщо бекенд вимагає
          токен у майбутньому, заголовок уже додається).
        </p>
      )}
    </div>
  )
}
