import { useEffect, useState, type FormEvent } from 'react'
import { Link, useParams } from 'react-router-dom'
import { apiJson } from '../api/http'
import type {
  CourseModuleResponse,
  CourseModuleSummaryResponse,
  CourseResponse,
  CreateCourseModuleRequest,
  CreateLessonRequest,
  LessonResponse,
  LessonSummaryResponse,
} from '../api/types'
import { useAuth } from '../auth/AuthContext'

export function CoursePage() {
  const { courseId } = useParams<{ courseId: string }>()
  const { accessToken } = useAuth()
  const [course, setCourse] = useState<CourseResponse | null>(null)
  const [modules, setModules] = useState<CourseModuleSummaryResponse[] | null>(null)
  const [moduleId, setModuleId] = useState<string | null>(null)
  const [lessons, setLessons] = useState<LessonSummaryResponse[] | null>(null)
  const [lessonId, setLessonId] = useState<string | null>(null)
  const [lesson, setLesson] = useState<LessonResponse | null>(null)
  const [error, setError] = useState<string | null>(null)
  const [modTitle, setModTitle] = useState('')
  const [modDesc, setModDesc] = useState('')
  const [lesTitle, setLesTitle] = useState('')
  const [lesContent, setLesContent] = useState('')
  const [busy, setBusy] = useState(false)

  useEffect(() => {
    if (!courseId) return
    let cancelled = false
    ;(async () => {
      setError(null)
      try {
        const [c, m] = await Promise.all([
          apiJson<CourseResponse>(`/api/courses/${courseId}`),
          apiJson<CourseModuleSummaryResponse[]>(`/api/courses/${courseId}/modules`),
        ])
        if (!cancelled) {
          setCourse(c)
          setModules(m)
          setModuleId(null)
          setLessons(null)
          setLessonId(null)
          setLesson(null)
        }
      } catch (e: unknown) {
        if (!cancelled) setError(e instanceof Error ? e.message : 'Помилка')
      }
    })()
    return () => {
      cancelled = true
    }
  }, [courseId])

  useEffect(() => {
    if (!courseId || !moduleId) {
      setLessons(null)
      return
    }
    let cancelled = false
    ;(async () => {
      try {
        const list = await apiJson<LessonSummaryResponse[]>(
          `/api/courses/${courseId}/modules/${moduleId}/lessons`,
        )
        if (!cancelled) {
          setLessons(list)
          setLessonId(null)
          setLesson(null)
        }
      } catch (e: unknown) {
        if (!cancelled) setError(e instanceof Error ? e.message : 'Помилка уроків')
      }
    })()
    return () => {
      cancelled = true
    }
  }, [courseId, moduleId])

  useEffect(() => {
    if (!courseId || !moduleId || !lessonId) {
      setLesson(null)
      return
    }
    let cancelled = false
    ;(async () => {
      try {
        const l = await apiJson<LessonResponse>(
          `/api/courses/${courseId}/modules/${moduleId}/lessons/${lessonId}`,
        )
        if (!cancelled) setLesson(l)
      } catch (e: unknown) {
        if (!cancelled) setError(e instanceof Error ? e.message : 'Помилка уроку')
      }
    })()
    return () => {
      cancelled = true
    }
  }, [courseId, moduleId, lessonId])

  async function addModule(e: FormEvent) {
    e.preventDefault()
    if (!courseId) return
    setBusy(true)
    setError(null)
    try {
      const body: CreateCourseModuleRequest = { title: modTitle, description: modDesc }
      await apiJson<CourseModuleResponse>(`/api/courses/${courseId}/modules`, {
        method: 'POST',
        body: JSON.stringify(body),
        accessToken,
      })
      const m = await apiJson<CourseModuleSummaryResponse[]>(`/api/courses/${courseId}/modules`)
      setModules(m)
      setModTitle('')
      setModDesc('')
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Не вдалося створити модуль')
    } finally {
      setBusy(false)
    }
  }

  async function addLesson(e: FormEvent) {
    e.preventDefault()
    if (!courseId || !moduleId) return
    setBusy(true)
    setError(null)
    try {
      const body: CreateLessonRequest = { title: lesTitle, content: lesContent }
      await apiJson<LessonResponse>(
        `/api/courses/${courseId}/modules/${moduleId}/lessons`,
        {
          method: 'POST',
          body: JSON.stringify(body),
          accessToken,
        },
      )
      const list = await apiJson<LessonSummaryResponse[]>(
        `/api/courses/${courseId}/modules/${moduleId}/lessons`,
      )
      setLessons(list)
      setLesTitle('')
      setLesContent('')
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Не вдалося створити урок')
    } finally {
      setBusy(false)
    }
  }

  if (!courseId) {
    return (
      <div className="page">
        <p>Невірне посилання.</p>
      </div>
    )
  }

  return (
    <div className="page">
      <p className="breadcrumb">
        <Link to="/courses">← Усі курси</Link>
      </p>
      {error && <p className="error">{error}</p>}
      {!course && <p>Завантаження…</p>}
      {course && (
        <>
          <header className="course-header">
            <h1>{course.title}</h1>
            <p className="muted">
              {course.theme} · оновлено {new Date(course.updatedAt).toLocaleString('uk-UA')}
            </p>
            <p className="course-desc">{course.description}</p>
          </header>

          <div className="grid-2">
            <section className="panel">
              <h2>Модулі</h2>
              {modules && modules.length === 0 && (
                <p className="muted">Модулів ще немає.</p>
              )}
              {modules && modules.length > 0 && (
                <ul className="stacked-list">
                  {modules.map((m) => (
                    <li key={m.id}>
                      <button
                        type="button"
                        className={`list-btn ${moduleId === m.id ? 'active' : ''}`}
                        onClick={() => setModuleId(m.id)}
                      >
                        <span>{m.title}</span>
                        <span className="badge">{m.lessonsCount} уроків</span>
                      </button>
                    </li>
                  ))}
                </ul>
              )}

              {accessToken && (
                <form className="form tight-top" onSubmit={addModule}>
                  <h3 className="h3">Новий модуль</h3>
                  <label className="field">
                    <span>Назва</span>
                    <input value={modTitle} onChange={(e) => setModTitle(e.target.value)} required />
                  </label>
                  <label className="field">
                    <span>Опис</span>
                    <textarea
                      value={modDesc}
                      onChange={(e) => setModDesc(e.target.value)}
                      required
                      rows={2}
                    />
                  </label>
                  <button type="submit" className="btn btn-secondary" disabled={busy}>
                    Додати модуль
                  </button>
                </form>
              )}
            </section>

            <section className="panel">
              <h2>Уроки</h2>
              {!moduleId && <p className="muted">Оберіть модуль зліва.</p>}
              {moduleId && lessons === null && <p>Завантаження…</p>}
              {moduleId && lessons && lessons.length === 0 && (
                <p className="muted">У цьому модулі ще немає уроків.</p>
              )}
              {moduleId && lessons && lessons.length > 0 && (
                <ul className="stacked-list">
                  {lessons.map((l) => (
                    <li key={l.id}>
                      <button
                        type="button"
                        className={`list-btn ${lessonId === l.id ? 'active' : ''}`}
                        onClick={() => setLessonId(l.id)}
                      >
                        {l.title}
                      </button>
                    </li>
                  ))}
                </ul>
              )}

              {accessToken && moduleId && (
                <form className="form tight-top" onSubmit={addLesson}>
                  <h3 className="h3">Новий урок</h3>
                  <label className="field">
                    <span>Назва</span>
                    <input value={lesTitle} onChange={(e) => setLesTitle(e.target.value)} required />
                  </label>
                  <label className="field">
                    <span>Зміст</span>
                    <textarea
                      value={lesContent}
                      onChange={(e) => setLesContent(e.target.value)}
                      required
                      rows={4}
                    />
                  </label>
                  <button type="submit" className="btn btn-secondary" disabled={busy}>
                    Додати урок
                  </button>
                </form>
              )}

              {lesson && (
                <article className="lesson-view">
                  <h3>{lesson.title}</h3>
                  <div className="lesson-body">{lesson.content}</div>
                </article>
              )}
            </section>
          </div>
        </>
      )}
    </div>
  )
}
