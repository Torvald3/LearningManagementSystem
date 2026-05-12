import { Link, NavLink, Outlet } from 'react-router-dom'
import { useAuth } from '../auth/AuthContext'

export function Layout() {
  const { accessToken, logout } = useAuth()

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
            <button type="button" className="btn btn-ghost" onClick={logout}>
              Вийти
            </button>
          )}
        </nav>
      </header>
      <main className="main">
        <Outlet />
      </main>
    </div>
  )
}
