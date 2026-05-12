import { Link } from 'react-router-dom'
import { useAuth } from '../auth/AuthContext'

export function HomePage() {
  const { accessToken, userId } = useAuth()

  return (
    <div className="page">
      <h1>Навчальна система</h1>
      <p className="lead">
        Це простий фронтенд для вашого .NET API: перегляд курсів, модулі, уроки, вхід і
        реєстрація.
      </p>
      <div className="card-row">
        <Link to="/courses" className="card card-link">
          <h2>Курси</h2>
          <p>Список усіх курсів з бекенду.</p>
        </Link>
        {!accessToken ? (
          <Link to="/login" className="card card-link">
            <h2>Обліковий запис</h2>
            <p>Увійдіть або зареєструйтесь, щоб отримати токен (для захищених ендпоінтів).</p>
          </Link>
        ) : (
          <div className="card">
            <h2>Ви в системі</h2>
            <p>
              Ідентифікатор користувача з токена:{' '}
              <code>{userId ?? 'невідомо'}</code>
            </p>
          </div>
        )}
      </div>
    </div>
  )
}
