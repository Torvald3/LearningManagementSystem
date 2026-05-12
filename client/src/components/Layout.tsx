import { Link, NavLink, Outlet } from 'react-router-dom'
import { useEffect, useState } from 'react'
import { apiJson } from '../api/http'
import type { UserResponse } from '../api/types'
import { useAuth } from '../auth/AuthContext'

export function Layout() {
  const { accessToken, userId, logout } = useAuth()
  const [username, setUsername] = useState<string | null>(null)

  useEffect(() => {
    let cancelled = false

    async function loadUser() {
      if (!accessToken || !userId) {
        setUsername(null)
        return
      }

      try {
        const user = await apiJson<UserResponse>(`/api/users/${userId}`, { accessToken })
        if (!cancelled) setUsername(user.username)
      } catch {
        if (!cancelled) setUsername(null)
      }
    }

    void loadUser()

    return () => {
      cancelled = true
    }
  }, [accessToken, userId])

  return (
    <div className="layout">
      <header className="header">
        <Link to="/" className="brand">
          LMS
        </Link>
        <nav className="nav">
          <NavLink
            to="/courses"
            className={({ isActive }) => `nav-link${isActive ? ' nav-link-active' : ''}`}
          >
            Курси
          </NavLink>
          {!accessToken && (
            <>
              <NavLink
                to="/login"
                className={({ isActive }) => `nav-link${isActive ? ' nav-link-active' : ''}`}
              >
                Увійти
              </NavLink>
              <NavLink
                to="/register"
                className={({ isActive }) =>
                  `nav-link nav-link-accent${isActive ? ' nav-link-active' : ''}`
                }
              >
                Реєстрація
              </NavLink>
            </>
          )}
          {accessToken && (
            <>
              {username && <span className="nav-user">{username}</span>}
              <button type="button" className="btn btn-ghost" onClick={logout}>
                Вийти
              </button>
            </>
          )}
        </nav>
      </header>
      <main className="main">
        <Outlet />
      </main>
    </div>
  )
}
