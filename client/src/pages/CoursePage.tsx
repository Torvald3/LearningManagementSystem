import { useCallback, useEffect, useState, type FormEvent } from 'react'
import { Link, useParams } from 'react-router-dom'
import { apiJson } from '../api/http'
import type {
  AddCourseMemberRequest,
  CourseMemberResponse,
  CourseModuleResponse,
  CourseModuleSummaryResponse,
  CourseResponse,
  CreateCourseModuleRequest,
  CreateLessonRequest,
  LessonResponse,
  LessonSummaryResponse,
  MediaResponse,
  MediaUrlResponse,
  UpdateCourseModuleRequest,
  UpdateLessonRequest,
  UserResponse,
} from '../api/types'
import { useAuth } from '../auth/AuthContext'

const MEMBER_ROLES = ['Teacher', 'Student'] as const

function formatFileSize(bytes: number) {
  if (bytes < 1024) return `${bytes} B`
  if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(1)} KiB`
  return `${(bytes / 1024 / 1024).toFixed(1)} MiB`
}

function toPosition(value: string, fallback: number) {
  const parsed = Number.parseInt(value, 10)
  return Number.isFinite(parsed) && parsed > 0 ? parsed : fallback
}

export function CoursePage() {
  const { courseId } = useParams<{ courseId: string }>()
  const { accessToken } = useAuth()
  const [course, setCourse] = useState<CourseResponse | null>(null)
  const [modules, setModules] = useState<CourseModuleSummaryResponse[] | null>(null)
  const [members, setMembers] = useState<CourseMemberResponse[] | null>(null)
  const [memberUsers, setMemberUsers] = useState<Record<string, UserResponse>>({})
  const [membersError, setMembersError] = useState<string | null>(null)
  const [moduleId, setModuleId] = useState<string | null>(null)
  const [lessons, setLessons] = useState<LessonSummaryResponse[] | null>(null)
  const [lessonId, setLessonId] = useState<string | null>(null)
  const [lesson, setLesson] = useState<LessonResponse | null>(null)
  const [lessonMedia, setLessonMedia] = useState<MediaResponse[] | null>(null)
  const [lessonMediaUrls, setLessonMediaUrls] = useState<Record<string, string>>({})
  const [lessonMediaError, setLessonMediaError] = useState<string | null>(null)
  const [selectedLessonMediaFile, setSelectedLessonMediaFile] = useState<File | null>(null)
  const [uploadingLessonMedia, setUploadingLessonMedia] = useState(false)
  const [lessonMediaInputKey, setLessonMediaInputKey] = useState(0)
  const [error, setError] = useState<string | null>(null)
  const [modTitle, setModTitle] = useState('')
  const [modDesc, setModDesc] = useState('')
  const [editingModuleId, setEditingModuleId] = useState<string | null>(null)
  const [editModTitle, setEditModTitle] = useState('')
  const [editModDesc, setEditModDesc] = useState('')
  const [editModPosition, setEditModPosition] = useState('1')
  const [lesTitle, setLesTitle] = useState('')
  const [lesContent, setLesContent] = useState('')
  const [editingLessonId, setEditingLessonId] = useState<string | null>(null)
  const [editLessonTitle, setEditLessonTitle] = useState('')
  const [editLessonContent, setEditLessonContent] = useState('')
  const [editLessonPosition, setEditLessonPosition] = useState('1')
  const [newMemberUserId, setNewMemberUserId] = useState('')
  const [newMemberRole, setNewMemberRole] = useState<string>('Student')
  const [busy, setBusy] = useState(false)

  const selectedModule = modules?.find((module) => module.id === moduleId) ?? null

  const refreshModules = useCallback(async () => {
    if (!courseId || !accessToken) return []

    const list = await apiJson<CourseModuleSummaryResponse[]>(`/api/courses/${courseId}/modules`, {
      accessToken,
    })
    setModules(list)
    return list
  }, [courseId, accessToken])

  const refreshLessons = useCallback(
    async (targetModuleId = moduleId) => {
      if (!courseId || !targetModuleId || !accessToken) return []

      const list = await apiJson<LessonSummaryResponse[]>(
        `/api/courses/${courseId}/modules/${targetModuleId}/lessons`,
        { accessToken },
      )
      setLessons(list)
      return list
    },
    [courseId, moduleId, accessToken],
  )

  const refreshMembers = useCallback(async () => {
    if (!courseId || !accessToken) return []

    const list = await apiJson<CourseMemberResponse[]>(`/api/courses/${courseId}/members`, {
      accessToken,
    })
    setMembers(list)
    setMembersError(null)
    return list
  }, [courseId, accessToken])

  const loadLessonMedia = useCallback(async () => {
    if (!lessonId || !accessToken) {
      setLessonMedia(null)
      setLessonMediaUrls({})
      return
    }

    setLessonMediaError(null)
    try {
      const list = await apiJson<MediaResponse[]>(
        `/api/media?entityType=Lesson&entityId=${lessonId}`,
        { accessToken },
      )
      setLessonMedia(list)

      const urlEntries = await Promise.all(
        list
          .filter((item) => item.contentType.startsWith('image/'))
          .map(async (item) => {
            const response = await apiJson<MediaUrlResponse>(`/api/media/${item.id}/url`, {
              accessToken,
            })
            return [item.id, response.url] as const
          }),
      )

      setLessonMediaUrls(Object.fromEntries(urlEntries))
    } catch (e: unknown) {
      setLessonMedia([])
      setLessonMediaUrls({})
      setLessonMediaError(e instanceof Error ? e.message : 'Не вдалося завантажити зображення')
    }
  }, [lessonId, accessToken])

  useEffect(() => {
    if (!courseId || !accessToken) return
    let cancelled = false
    ;(async () => {
      setError(null)
      setMembersError(null)
      try {
        const [loadedCourse, loadedModules] = await Promise.all([
          apiJson<CourseResponse>(`/api/courses/${courseId}`, { accessToken }),
          apiJson<CourseModuleSummaryResponse[]>(`/api/courses/${courseId}/modules`, {
            accessToken,
          }),
        ])
        if (!cancelled) {
          setCourse(loadedCourse)
          setModules(loadedModules)
          setModuleId(null)
          setLessons(null)
          setLessonId(null)
          setLesson(null)
          setLessonMedia(null)
          setLessonMediaUrls({})
        }
      } catch (e: unknown) {
        if (!cancelled) setError(e instanceof Error ? e.message : 'Помилка')
      }

      try {
        const loadedMembers = await refreshMembers()
        if (!cancelled) setMembers(loadedMembers)
      } catch (e: unknown) {
        if (!cancelled) {
          setMembers(null)
          setMembersError(e instanceof Error ? e.message : 'Немає доступу до списку учасників')
        }
      }
    })()
    return () => {
      cancelled = true
    }
  }, [courseId, accessToken, refreshMembers])

  useEffect(() => {
    if (!members || !accessToken) {
      setMemberUsers({})
      return
    }

    let cancelled = false
    ;(async () => {
      const entries = await Promise.all(
        members.map(async (member): Promise<[string, UserResponse | null]> => {
          try {
            const user = await apiJson<UserResponse>(`/api/users/${member.userId}`, {
              accessToken,
            })
            return [member.userId, user]
          } catch {
            return [member.userId, null]
          }
        }),
      )

      if (!cancelled) {
        const next: Record<string, UserResponse> = {}
        for (const [id, user] of entries) {
          if (user) next[id] = user
        }
        setMemberUsers(next)
      }
    })()

    return () => {
      cancelled = true
    }
  }, [members, accessToken])

  useEffect(() => {
    if (!courseId || !moduleId || !accessToken) {
      setLessons(null)
      setLessonMedia(null)
      setLessonMediaUrls({})
      return
    }
    let cancelled = false
    ;(async () => {
      try {
        const list = await apiJson<LessonSummaryResponse[]>(
          `/api/courses/${courseId}/modules/${moduleId}/lessons`,
          { accessToken },
        )
        if (!cancelled) {
          setLessons(list)
          setLessonId(null)
          setLesson(null)
          setEditingLessonId(null)
          setLessonMedia(null)
          setLessonMediaUrls({})
        }
      } catch (e: unknown) {
        if (!cancelled) setError(e instanceof Error ? e.message : 'Помилка уроків')
      }
    })()
    return () => {
      cancelled = true
    }
  }, [courseId, moduleId, accessToken])

  useEffect(() => {
    void loadLessonMedia()
  }, [loadLessonMedia])

  useEffect(() => {
    if (!courseId || !moduleId || !lessonId || !accessToken) {
      setLesson(null)
      return
    }
    let cancelled = false
    ;(async () => {
      try {
        const loadedLesson = await apiJson<LessonResponse>(
          `/api/courses/${courseId}/modules/${moduleId}/lessons/${lessonId}`,
          { accessToken },
        )
        if (!cancelled) {
          setLesson(loadedLesson)
          setEditingLessonId(null)
        }
      } catch (e: unknown) {
        if (!cancelled) setError(e instanceof Error ? e.message : 'Помилка уроку')
      }
    })()
    return () => {
      cancelled = true
    }
  }, [courseId, moduleId, lessonId, accessToken])

  async function addModule(e: FormEvent) {
    e.preventDefault()
    if (!courseId || !accessToken) return
    setBusy(true)
    setError(null)
    try {
      const body: CreateCourseModuleRequest = { title: modTitle, description: modDesc }
      await apiJson<CourseModuleResponse>(`/api/courses/${courseId}/modules`, {
        method: 'POST',
        body: JSON.stringify(body),
        accessToken,
      })
      await refreshModules()
      setModTitle('')
      setModDesc('')
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Не вдалося створити модуль')
    } finally {
      setBusy(false)
    }
  }

  function startEditModule(module: CourseModuleSummaryResponse) {
    setEditingModuleId(module.id)
    setEditModTitle(module.title)
    setEditModDesc(module.description)
    setEditModPosition(String(module.position))
  }

  function cancelEditModule() {
    setEditingModuleId(null)
    setEditModTitle('')
    setEditModDesc('')
    setEditModPosition('1')
  }

  async function updateModule(e: FormEvent) {
    e.preventDefault()
    if (!courseId || !accessToken || !editingModuleId) return
    const fallbackPosition =
      modules?.find((module) => module.id === editingModuleId)?.position ?? modules?.length ?? 1

    setBusy(true)
    setError(null)
    try {
      const body: UpdateCourseModuleRequest = {
        title: editModTitle,
        description: editModDesc,
        position: toPosition(editModPosition, fallbackPosition),
      }
      await apiJson<CourseModuleResponse>(`/api/courses/${courseId}/modules/${editingModuleId}`, {
        method: 'PUT',
        body: JSON.stringify(body),
        accessToken,
      })
      await refreshModules()
      cancelEditModule()
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Не вдалося оновити модуль')
    } finally {
      setBusy(false)
    }
  }

  async function archiveModule(module: CourseModuleSummaryResponse) {
    if (!courseId || !accessToken) return
    if (!window.confirm(`Видалити модуль "${module.title}"?`)) return

    setBusy(true)
    setError(null)
    try {
      await apiJson<void>(`/api/courses/${courseId}/modules/${module.id}`, {
        method: 'DELETE',
        accessToken,
      })
      await refreshModules()
      if (moduleId === module.id) {
        setModuleId(null)
        setLessons(null)
        setLessonId(null)
        setLesson(null)
        setLessonMedia(null)
        setLessonMediaUrls({})
      }
      if (editingModuleId === module.id) cancelEditModule()
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Не вдалося видалити модуль')
    } finally {
      setBusy(false)
    }
  }

  async function addLesson(e: FormEvent) {
    e.preventDefault()
    if (!courseId || !moduleId || !accessToken) return
    setBusy(true)
    setError(null)
    try {
      const body: CreateLessonRequest = { title: lesTitle, content: lesContent }
      await apiJson<LessonResponse>(`/api/courses/${courseId}/modules/${moduleId}/lessons`, {
        method: 'POST',
        body: JSON.stringify(body),
        accessToken,
      })
      await refreshLessons(moduleId)
      await refreshModules()
      setLesTitle('')
      setLesContent('')
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Не вдалося створити урок')
    } finally {
      setBusy(false)
    }
  }

  function startEditLesson() {
    if (!lesson) return
    setEditingLessonId(lesson.id)
    setEditLessonTitle(lesson.title)
    setEditLessonContent(lesson.content)
    setEditLessonPosition(String(lesson.position))
  }

  function cancelEditLesson() {
    setEditingLessonId(null)
    setEditLessonTitle('')
    setEditLessonContent('')
    setEditLessonPosition('1')
  }

  async function updateLesson(e: FormEvent) {
    e.preventDefault()
    if (!courseId || !moduleId || !lessonId || !lesson || !accessToken) return

    setBusy(true)
    setError(null)
    try {
      const body: UpdateLessonRequest = {
        title: editLessonTitle,
        content: editLessonContent,
        position: toPosition(editLessonPosition, lesson.position),
      }
      const updated = await apiJson<LessonResponse>(
        `/api/courses/${courseId}/modules/${moduleId}/lessons/${lessonId}`,
        {
          method: 'PUT',
          body: JSON.stringify(body),
          accessToken,
        },
      )
      setLesson(updated)
      await refreshLessons(moduleId)
      cancelEditLesson()
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Не вдалося оновити урок')
    } finally {
      setBusy(false)
    }
  }

  async function archiveLesson() {
    if (!courseId || !moduleId || !lessonId || !lesson || !accessToken) return
    if (!window.confirm(`Видалити урок "${lesson.title}"?`)) return

    setBusy(true)
    setError(null)
    try {
      await apiJson<void>(`/api/courses/${courseId}/modules/${moduleId}/lessons/${lessonId}`, {
        method: 'DELETE',
        accessToken,
      })
      await refreshLessons(moduleId)
      await refreshModules()
      setLessonId(null)
      setLesson(null)
      setEditingLessonId(null)
      setLessonMedia(null)
      setLessonMediaUrls({})
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Не вдалося видалити урок')
    } finally {
      setBusy(false)
    }
  }

  async function addMember(e: FormEvent) {
    e.preventDefault()
    if (!courseId || !accessToken || !newMemberUserId.trim()) return
    setBusy(true)
    setError(null)
    try {
      const body: AddCourseMemberRequest = {
        userId: newMemberUserId.trim(),
        role: newMemberRole,
      }
      await apiJson<CourseMemberResponse>(`/api/courses/${courseId}/members`, {
        method: 'POST',
        body: JSON.stringify(body),
        accessToken,
      })
      await refreshMembers()
      setNewMemberUserId('')
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Не вдалося додати учасника')
    } finally {
      setBusy(false)
    }
  }

  async function removeMember(member: CourseMemberResponse) {
    if (!courseId || !accessToken) return
    const memberName = memberUsers[member.userId]?.username ?? 'учасника'
    if (!window.confirm(`Видалити ${memberName} з курсу?`)) return

    setBusy(true)
    setError(null)
    try {
      await apiJson<void>(`/api/courses/${courseId}/members/${member.userId}`, {
        method: 'DELETE',
        accessToken,
      })
      await refreshMembers()
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Не вдалося видалити учасника')
    } finally {
      setBusy(false)
    }
  }

  async function uploadLessonMedia(e: FormEvent) {
    e.preventDefault()
    if (!lessonId || !accessToken || !selectedLessonMediaFile) return

    setUploadingLessonMedia(true)
    setLessonMediaError(null)
    try {
      const body = new FormData()
      body.set('file', selectedLessonMediaFile)
      body.set('entityType', 'Lesson')
      body.set('entityId', lessonId)

      await apiJson<MediaResponse>('/api/media', {
        method: 'POST',
        body,
        accessToken,
      })

      setSelectedLessonMediaFile(null)
      setLessonMediaInputKey((value) => value + 1)
      await loadLessonMedia()
    } catch (err: unknown) {
      setLessonMediaError(err instanceof Error ? err.message : 'Не вдалося завантажити зображення')
    } finally {
      setUploadingLessonMedia(false)
    }
  }

  async function archiveLessonMedia(mediaId: string) {
    if (!accessToken) return

    setLessonMediaError(null)
    try {
      await apiJson<void>(`/api/media/${mediaId}`, {
        method: 'DELETE',
        accessToken,
      })
      setLessonMedia((prev) => prev?.filter((item) => item.id !== mediaId) ?? prev)
      setLessonMediaUrls((prev) => {
        const next = { ...prev }
        delete next[mediaId]
        return next
      })
    } catch (err: unknown) {
      setLessonMediaError(err instanceof Error ? err.message : 'Не вдалося видалити зображення')
    }
  }

  if (!courseId) {
    return (
      <div className="page">
        <p>Невірне посилання.</p>
      </div>
    )
  }

  if (!accessToken) {
    return (
      <div className="page">
        <p className="breadcrumb">
          <Link to="/courses">← Курси</Link>
        </p>
        <h1>Курс</h1>
        <p className="lead">Перегляд курсу доступний лише після входу.</p>
        <Link to="/login" className="btn btn-primary">
          Увійти
        </Link>
      </div>
    )
  }

  return (
    <div className="page course-page">
      <p className="breadcrumb">
        <Link to="/courses">← Усі курси</Link>
      </p>
      {error && <p className="error">{error}</p>}
      {!course && <p>Завантаження...</p>}
      {course && (
        <>
          <header className="course-header">
            <h1>{course.title}</h1>
            <p className="muted">
              {course.theme} · оновлено {new Date(course.updatedAt).toLocaleString('uk-UA')}
            </p>
            <p className="course-desc">{course.description}</p>
          </header>

          <div className="course-workspace">
            <aside className="course-sidebar">
              <section className="panel course-nav-panel">
                <h2>Модулі</h2>
                {modules && modules.length === 0 && (
                  <p className="muted">Модулів ще немає.</p>
                )}
                {modules && modules.length > 0 && (
                  <ul className="module-rail">
                    {modules.map((module) => (
                      <li className="module-rail-item" key={module.id}>
                        <div className="module-row">
                          <button
                            type="button"
                            className={`module-select ${moduleId === module.id ? 'active' : ''}`}
                            onClick={() => setModuleId(module.id)}
                          >
                            <span>{module.title}</span>
                            <span className="badge">{module.lessonsCount} уроків</span>
                          </button>
                          <div className="item-actions">
                            <button
                              type="button"
                              className="mini-btn"
                              onClick={() => startEditModule(module)}
                              title="Редагувати модуль"
                            >
                              Ред.
                            </button>
                            <button
                              type="button"
                              className="mini-btn mini-btn-danger"
                              onClick={() => void archiveModule(module)}
                              title="Видалити модуль"
                            >
                              Вид.
                            </button>
                          </div>
                        </div>

                        {moduleId === module.id && (
                          <div className="lesson-rail-wrap">
                            {lessons === null && <p className="muted small">Уроки...</p>}
                            {lessons && lessons.length === 0 && (
                              <p className="muted small">Уроків ще немає.</p>
                            )}
                            {lessons && lessons.length > 0 && (
                              <ul className="lesson-rail">
                                {lessons.map((item) => (
                                  <li key={item.id}>
                                    <button
                                      type="button"
                                      className={`lesson-select ${
                                        lessonId === item.id ? 'active' : ''
                                      }`}
                                      onClick={() => setLessonId(item.id)}
                                    >
                                      {item.title}
                                    </button>
                                  </li>
                                ))}
                              </ul>
                            )}
                          </div>
                        )}
                      </li>
                    ))}
                  </ul>
                )}

                {editingModuleId ? (
                  <form className="form compact-form tight-top" onSubmit={updateModule}>
                    <h3 className="h3">Редагувати модуль</h3>
                    <label className="field">
                      <span>Назва</span>
                      <input
                        value={editModTitle}
                        onChange={(e) => setEditModTitle(e.target.value)}
                        required
                      />
                    </label>
                    <label className="field">
                      <span>Опис</span>
                      <textarea
                        value={editModDesc}
                        onChange={(e) => setEditModDesc(e.target.value)}
                        required
                        rows={3}
                      />
                    </label>
                    <label className="field">
                      <span>Позиція</span>
                      <input
                        type="number"
                        min="1"
                        max={modules?.length ?? 1}
                        value={editModPosition}
                        onChange={(e) => setEditModPosition(e.target.value)}
                        required
                      />
                    </label>
                    <div className="form-actions">
                      <button type="submit" className="btn btn-secondary" disabled={busy}>
                        Зберегти
                      </button>
                      <button type="button" className="btn btn-ghost" onClick={cancelEditModule}>
                        Скасувати
                      </button>
                    </div>
                  </form>
                ) : (
                  <form className="form compact-form tight-top" onSubmit={addModule}>
                    <h3 className="h3">Новий модуль</h3>
                    <label className="field">
                      <span>Назва</span>
                      <input
                        value={modTitle}
                        onChange={(e) => setModTitle(e.target.value)}
                        required
                      />
                    </label>
                    <label className="field">
                      <span>Опис</span>
                      <textarea
                        value={modDesc}
                        onChange={(e) => setModDesc(e.target.value)}
                        required
                        rows={3}
                      />
                    </label>
                    <button type="submit" className="btn btn-secondary" disabled={busy}>
                      Додати модуль
                    </button>
                  </form>
                )}

                {moduleId && (
                  <form className="form compact-form tight-top" onSubmit={addLesson}>
                    <h3 className="h3">Новий урок</h3>
                    <label className="field">
                      <span>Назва</span>
                      <input
                        value={lesTitle}
                        onChange={(e) => setLesTitle(e.target.value)}
                        required
                      />
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
              </section>

              <section className="panel course-members-panel">
                <h2>Учасники</h2>
                {membersError && <p className="muted">{membersError}</p>}
                {members && members.length === 0 && (
                  <p className="muted">Поки що немає записів.</p>
                )}
                {members && members.length > 0 && (
                  <ul className="member-list member-list-names">
                    {members.map((member) => (
                      <li key={member.id}>
                        <div className="member-main">
                          <span className="member-name">
                            {memberUsers[member.userId]?.username ?? 'Користувач'}
                          </span>
                          <span className="member-role">{member.role}</span>
                        </div>
                        {member.role !== 'CourseOwner' && (
                          <button
                            type="button"
                            className="mini-btn mini-btn-danger"
                            onClick={() => void removeMember(member)}
                            disabled={busy}
                            title="Видалити учасника"
                          >
                            Вид.
                          </button>
                        )}
                      </li>
                    ))}
                  </ul>
                )}

                <form className="form compact-form tight-top" onSubmit={addMember}>
                  <label className="field">
                    <span>UserId нового учасника (GUID)</span>
                    <input
                      value={newMemberUserId}
                      onChange={(e) => setNewMemberUserId(e.target.value)}
                      placeholder="00000000-0000-0000-0000-000000000000"
                    />
                  </label>
                  <label className="field">
                    <span>Роль</span>
                    <select
                      value={newMemberRole}
                      onChange={(e) => setNewMemberRole(e.target.value)}
                    >
                      {MEMBER_ROLES.map((role) => (
                        <option key={role} value={role}>
                          {role}
                        </option>
                      ))}
                    </select>
                  </label>
                  <button type="submit" className="btn btn-secondary" disabled={busy}>
                    Додати
                  </button>
                </form>
              </section>
            </aside>

            <section className="panel lesson-panel">
              {!moduleId && <p className="muted">Оберіть модуль зліва.</p>}
              {moduleId && !lesson && (
                <div className="empty-lesson">
                  <p className="muted">
                    {selectedModule
                      ? `Оберіть урок у модулі "${selectedModule.title}".`
                      : 'Оберіть урок.'}
                  </p>
                </div>
              )}

              {lesson && (
                <>
                  <div className="lesson-panel-header">
                    <div>
                      <p className="eyebrow">{selectedModule?.title ?? 'Урок'}</p>
                      <h2>{lesson.title}</h2>
                    </div>
                    <div className="lesson-actions">
                      <button
                        type="button"
                        className="btn btn-secondary"
                        onClick={startEditLesson}
                        disabled={busy}
                      >
                        Редагувати
                      </button>
                      <button
                        type="button"
                        className="btn btn-ghost danger-action"
                        onClick={() => void archiveLesson()}
                        disabled={busy}
                      >
                        Видалити
                      </button>
                    </div>
                  </div>

                  {editingLessonId === lesson.id ? (
                    <form className="form lesson-edit-form" onSubmit={updateLesson}>
                      <label className="field">
                        <span>Назва</span>
                        <input
                          value={editLessonTitle}
                          onChange={(e) => setEditLessonTitle(e.target.value)}
                          required
                        />
                      </label>
                      <label className="field">
                        <span>Зміст</span>
                        <textarea
                          value={editLessonContent}
                          onChange={(e) => setEditLessonContent(e.target.value)}
                          required
                          rows={10}
                        />
                      </label>
                      <label className="field position-field">
                        <span>Позиція</span>
                        <input
                          type="number"
                          min="1"
                          max={lessons?.length ?? 1}
                          value={editLessonPosition}
                          onChange={(e) => setEditLessonPosition(e.target.value)}
                          required
                        />
                      </label>
                      <div className="form-actions">
                        <button type="submit" className="btn btn-secondary" disabled={busy}>
                          Зберегти
                        </button>
                        <button type="button" className="btn btn-ghost" onClick={cancelEditLesson}>
                          Скасувати
                        </button>
                      </div>
                    </form>
                  ) : (
                    <article className="lesson-view">
                      <div className="lesson-body">{lesson.content}</div>
                    </article>
                  )}

                  <section className="lesson-media">
                    <div className="lesson-media-header">
                      <h3 className="h3">Зображення уроку</h3>
                      {lessonMedia && lessonMedia.length > 0 && (
                        <span className="muted small">{lessonMedia.length} файлів</span>
                      )}
                    </div>
                    {lessonMediaError && <p className="error">{lessonMediaError}</p>}
                    <form className="form form-inline media-upload-form" onSubmit={uploadLessonMedia}>
                      <label className="field field-grow">
                        <span>Файл</span>
                        <input
                          key={lessonMediaInputKey}
                          type="file"
                          accept="image/*"
                          onChange={(e) => setSelectedLessonMediaFile(e.target.files?.[0] ?? null)}
                        />
                      </label>
                      <button
                        type="submit"
                        className="btn btn-secondary"
                        disabled={uploadingLessonMedia || !selectedLessonMediaFile}
                      >
                        {uploadingLessonMedia ? 'Завантаження...' : 'Завантажити'}
                      </button>
                    </form>

                    {lessonMedia === null && <p>Завантаження...</p>}
                    {lessonMedia && lessonMedia.length === 0 && (
                      <p className="muted">Для цього уроку ще немає зображень.</p>
                    )}
                    {lessonMedia && lessonMedia.length > 0 && (
                      <div className="media-grid">
                        {lessonMedia.map((item) => (
                          <article className="media-card" key={item.id}>
                            {lessonMediaUrls[item.id] ? (
                              <img
                                className="media-thumb"
                                src={lessonMediaUrls[item.id]}
                                alt={item.originalFileName}
                              />
                            ) : (
                              <div className="media-thumb media-thumb-empty">Без прев'ю</div>
                            )}
                            <div className="media-meta">
                              <strong>{item.originalFileName}</strong>
                              <span className="muted small">
                                {formatFileSize(item.size)} ·{' '}
                                {new Date(item.createdAt).toLocaleString('uk-UA')}
                              </span>
                            </div>
                            <button
                              type="button"
                              className="btn btn-ghost"
                              onClick={() => void archiveLessonMedia(item.id)}
                            >
                              Прибрати
                            </button>
                          </article>
                        ))}
                      </div>
                    )}
                  </section>
                </>
              )}
            </section>
          </div>
        </>
      )}
    </div>
  )
}
