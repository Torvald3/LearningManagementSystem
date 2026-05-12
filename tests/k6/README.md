# k6 Load Testing

Цей набір перевіряє NFR/SLO для LMS через реальні API-потоки: login, список курсів,
завантаження сторінки курсу та створення навчального контенту.

> Примітка: у k6 кастомна latency-метрика називається `Trend`, тому в сценаріях
> використано `Trend as Histogram`. Це дає робочий аналог гістограми й водночас
> лишає назву `Histogram` у коді для відповідності вимогам лабораторної.

## Передумови

Запустити інфраструктуру та API:

```powershell
docker compose up -d
dotnet run --project src/LMS.App/LMS.App.csproj --launch-profile http
```

За замовчуванням тести використовують:

```text
BASE_URL=http://localhost:5277
```

Можна перевизначити:

```powershell
$env:BASE_URL="http://localhost:5277"
$env:K6_TEST_EMAIL="existing-confirmed-user@example.com"
$env:K6_TEST_PASSWORD="K6Password!2026"
```

Якщо `K6_TEST_EMAIL` не задано, кожен сценарій сам створить користувача через
`POST /api/auth/register`, підтвердить email через `POST /api/auth/confirm-email`
і отримає JWT через `POST /api/auth/login`.

## Сценарії

### SCN-01: Smoke - базова перевірка LMS

Тип: Smoke  
Endpoint(s): `POST /api/auth/login`, `GET /api/auth/test`, `GET /api/courses`, `GET /metrics`  
Профіль: `1 VU, 30s`  
Пов'язана NFR: `NFR-01 Login Latency`, `NFR-09 Availability`  
Критерій успіху: login `p95 < 500ms`, `http_req_failed < 1%`, `checks > 99%`

```powershell
k6 run tests/k6/scenarios/01-smoke.js
```

### SCN-02: Load - Login Latency

Тип: Load  
Endpoint(s): `POST /api/auth/login`  
Профіль: `0 -> 50 VUs за 1m`, `50 VUs протягом 3m`, `50 -> 0 VUs за 1m`  
Пов'язана NFR: `NFR-01 Login response time p95 <= 500ms`  
Критерій успіху: login `p95 < 500ms`, login errors `< 0.5%`, `checks > 99%`

```powershell
k6 run tests/k6/scenarios/02-login-load.js
```

### SCN-03: Load - Course List Loading

Тип: Load  
Endpoint(s): `GET /api/courses`, `GET /api/courses/my/learning`, `GET /api/courses/my/teaching`  
Профіль: `0 -> 100 VUs за 2m`, `100 VUs протягом 5m`, `100 -> 0 VUs за 1m`  
Пов'язана NFR: `NFR-02 Course list <= 800ms`  
Критерій успіху: course-list `p95 < 800ms`, errors `< 0.5%`, `checks > 99%`

```powershell
k6 run tests/k6/scenarios/03-course-list-load.js
```

### SCN-04: Spike/Scalability - Course Feed Loading

Тип: Spike / Load  
Endpoint(s): `GET /api/courses/{courseId}`, `GET /api/courses/{courseId}/modules`,
`GET /api/courses/{courseId}/modules/{moduleId}`, `GET /api/courses/{courseId}/modules/{moduleId}/lessons`,
`GET /api/courses/{courseId}/modules/{moduleId}/lessons/{lessonId}`  
Профіль: `0 -> 100 VUs за 1m`, `100 -> 500 VUs за 3m`, `500 VUs протягом 5m`, `500 -> 0 VUs за 1m`  
Пов'язана NFR: `NFR-03 Course feed <= 1000ms`, `NFR-15 at least 500 concurrent users`  
Критерій успіху: feed `p95 < 1000ms`, errors `< 1%`, `checks > 98%`, `vus_max >= 500`

```powershell
k6 run tests/k6/scenarios/04-course-feed-spike.js
```

### SCN-05: Stress - Course Creation

Тип: Stress  
Endpoint(s): `POST /api/courses`, `POST /api/courses/{courseId}/modules`,
`POST /api/courses/{courseId}/modules/{moduleId}/lessons`  
Профіль: `0 -> 20 VUs за 1m`, `20 -> 50 VUs за 2m`, `50 -> 100 VUs за 2m`, `100 -> 0 VUs за 1m`  
Пов'язана NFR: `NFR-04 Course creation <= 500ms`, `NFR-10 atomic database operations`  
Критерій успіху: course creation `p95 < 500ms`, errors `< 1%`, `checks > 98%`

```powershell
k6 run tests/k6/scenarios/05-course-creation-stress.js
```

## NFR-05 Assignment Creation

У поточному API немає endpoint-ів для assignment creation. Для звіту варто зазначити,
що `NFR-05` не автоматизовано через відсутність реалізованого Assignment API. Після
додавання endpoint-а сценарій треба зробити аналогічно до `05-course-creation-stress.js`,
але з threshold `p95 < 700ms`.
