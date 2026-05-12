import { Link } from 'react-router-dom'
import { useAuth } from '../auth/AuthContext'

export function HomePage() {
  const { accessToken, userId } = useAuth()

  return (
    <div className="page">
      <h1>Навчальна система</h1>
      <p className="lead">
        Фронтенд для вашого LMS API: курси (лише після входу), ролі студент / викладач, учасники
        курсу, модулі та уроки, реєстрація та вхід.
      </p>
      <div className="card-row">
        <Link to="/courses" className="card card-link">
          <h2>Курси</h2>
          <p>Списки за участю: усі мої, навчання, викладання (потрібен вхід).</p>
        </Link>
        {!accessToken ? (
          <Link to="/login" className="card card-link">
            <h2>Обліковий запис</h2>
            <p>Увійдіть або зареєструйтесь: без токена API курсів не віддає дані.</p>
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
