import { useCallback, useEffect, useState, type FormEvent } from 'react'
import { Link } from 'react-router-dom'
import { apiJson } from '../api/http'
import type { CourseResponse, CreateCourseRequest } from '../api/types'
import { useAuth } from '../auth/AuthContext'

type CoursesTab = 'member' | 'learning' | 'teaching'

const TAB_PATH: Record<CoursesTab, string> = {
  member: '/api/courses',
  learning: '/api/courses/my/learning',
  teaching: '/api/courses/my/teaching',
}

const TAB_LABEL: Record<CoursesTab, string> = {
  member: 'Усі мої курси',
  learning: 'Навчаюсь (студент)',
  teaching: 'Викладаю (власник / викладач)',
}

export function CoursesPage() {
  const { accessToken } = useAuth()
  const [tab, setTab] = useState<CoursesTab>('member')
  const [courses, setCourses] = useState<CourseResponse[] | null>(null)
  const [error, setError] = useState<string | null>(null)
  const [title, setTitle] = useState('')
  const [theme, setTheme] = useState('')
  const [description, setDescription] = useState('')
  const [creating, setCreating] = useState(false)

  const loadCourses = useCallback(async () => {
    if (!accessToken) {
      setCourses(null)
      return
    }
    setError(null)
    setCourses(null)
    try {
      const list = await apiJson<CourseResponse[]>(TAB_PATH[tab], { accessToken })
      setCourses(list)
    } catch (e: unknown) {
      setError(e instanceof Error ? e.message : 'Не вдалося завантажити курси')
      setCourses([])
    }
  }, [accessToken, tab])

  useEffect(() => {
    void loadCourses()
  }, [loadCourses])

  async function createCourse(e: FormEvent) {
    e.preventDefault()
    if (!accessToken) return
    setError(null)
    setCreating(true)
    try {
      const body: CreateCourseRequest = {
        title,
        theme,
        description,
      }
      await apiJson<CourseResponse>('/api/courses', {
        method: 'POST',
        body: JSON.stringify(body),
        accessToken,
      })
      setTitle('')
      setTheme('')
      setDescription('')
      await loadCourses()
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Помилка створення')
    } finally {
      setCreating(false)
    }
  }

  if (!accessToken) {
    return (
      <div className="page">
        <h1>Курси</h1>
        <p className="lead">
          Ендпоінти курсів тепер вимагають авторизацію. Увійдіть, щоб переглядати курси, де ви учасник,
          окремо — як студент або як викладач / власник.
        </p>
        <Link to="/login" className="btn btn-primary">
          Увійти
        </Link>
      </div>
    )
  }

  return (
    <div className="page">
      <h1>Курси</h1>
      <p className="muted small">
        Список залежить від обраної вкладки: усі курси, де ви в команді; лише як студент; лише як
        власник або викладач.
      </p>

      <div className="tab-row" role="tablist">
        {(Object.keys(TAB_PATH) as CoursesTab[]).map((key) => (
          <button
            key={key}
            type="button"
            role="tab"
            aria-selected={tab === key}
            className={`tab${tab === key ? ' tab-active' : ''}`}
            onClick={() => setTab(key)}
          >
            {TAB_LABEL[key]}
          </button>
        ))}
      </div>

      {error && <p className="error">{error}</p>}
      {courses === null && <p>Завантаження…</p>}
      {courses && courses.length === 0 && <p className="muted">У цьому списку поки що немає курсів.</p>}
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

      {accessToken && (
        <section className="panel">
          <h2>Створити курс</h2>
          <p className="muted small">
            Автор фіксується з вашого JWT; тіло запиту містить лише назву, тему й опис.
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
    </div>
  )
}
