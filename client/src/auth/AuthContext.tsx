import {
  createContext,
  useCallback,
  useContext,
  useMemo,
  useState,
  type ReactNode,
} from 'react'
import { readJwtSubject } from './jwt'

const STORAGE_KEY = 'lms_access_token'

type AuthContextValue = {
  accessToken: string | null
  userId: string | null
  setAccessToken: (token: string | null) => void
  logout: () => void
}

const AuthContext = createContext<AuthContextValue | null>(null)

export function AuthProvider({ children }: { children: ReactNode }) {
  const [accessToken, setTokenState] = useState<string | null>(() =>
    typeof window !== 'undefined' ? localStorage.getItem(STORAGE_KEY) : null,
  )

  const setAccessToken = useCallback((token: string | null) => {
    setTokenState(token)
    if (token) {
      localStorage.setItem(STORAGE_KEY, token)
    } else {
      localStorage.removeItem(STORAGE_KEY)
    }
  }, [])

  const logout = useCallback(() => {
    setAccessToken(null)
  }, [setAccessToken])

  const userId = useMemo(
    () => (accessToken ? readJwtSubject(accessToken) : null),
    [accessToken],
  )

  const value = useMemo(
    () => ({ accessToken, userId, setAccessToken, logout }),
    [accessToken, userId, setAccessToken, logout],
  )

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

export function useAuth(): AuthContextValue {
  const ctx = useContext(AuthContext)
  if (!ctx) {
    throw new Error('useAuth must be used within AuthProvider')
  }
  return ctx
}
