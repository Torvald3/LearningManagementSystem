# k6 Load Testing Report

## Test Environment

- API: `http://localhost:5277`
- Database: PostgreSQL via Docker, port `5433`
- Object storage: MinIO via Docker, port `9000`
- Tool: k6
- Date/time: 2026-05-12

## Results

### RES-01: Smoke - базова перевірка LMS

Дата/час: 2026-05-12  
Тривалість: 31.0 сек  
Max VUs: 1  
Запитів: 119  
RPS: 3.84 avg  
p50: 1.52ms  
p95: 60.22ms; Smoke_Login p95 = 62.07ms  
p99: не виведено у стандартному k6 summary  
Error rate: 0.00%  
Thresholds:

- `checks rate > 99%`: 100.00% - виконується
- `Smoke_Login p95 < 500ms`: 62.07ms - виконується
- `http_req_failed < 1%`: 0.00% - виконується

Висновок: NFR-01 та NFR-09 виконуються у smoke-перевірці. API доступний, login працює, авторизований endpoint і `/metrics` відповідають без помилок.

### RES-02: Load - Login Latency

Дата/час: 2026-05-12  
Тривалість: 5 хв 00.4 сек  
Max VUs: 50  
Запитів: 11 230  
RPS: 37.39 avg  
p50: 66.51ms  
p95: 112.86ms; Login_Load p95 = 112.88ms  
p99: не виведено у стандартному k6 summary  
Error rate: 0.00%  
Thresholds:

- `checks rate > 99%`: 100.00% - виконується
- `Login_Load p95 < 500ms`: 112.88ms - виконується
- `Login_Load errors < 0.5%`: 0.00% - виконується

Висновок: NFR-01 виконується при навантаженні до 50 VUs. Login має значний запас: p95 = 112.88ms при цільовому значенні <= 500ms.

### RES-03: Load - Course List Loading

Дата/час: 2026-05-12  
Тривалість: 8 хв 01.4 сек  
Max VUs: 100  
Запитів: 116 061  
RPS: 241.07 avg  
p50: 2.87ms  
p95: 5.36ms  
p99: не виведено у стандартному k6 summary  
Error rate: 0.00%  
Thresholds:

- `checks rate > 99%`: 100.00% - виконується
- `NFR-02 p95 < 800ms`: 5.36ms - виконується
- `NFR-02 errors < 0.5%`: 0.00% - виконується

Висновок: NFR-02 виконується при навантаженні до 100 VUs. Список курсів завантажується значно швидше за цільові 800ms, помилок не зафіксовано.

### RES-04: Spike/Scalability - Course Feed Loading

Дата/час: 2026-05-12  
Тривалість: 10 хв 01.0 сек  
Max VUs: 500  
Запитів: 453 416  
RPS: 754.40 avg  
p50: 183.17ms  
p95: 846.91ms  
p99: не виведено у стандартному k6 summary  
Error rate: 0.00%  
Thresholds:

- `checks rate > 98%`: 98.27% - виконується
- `NFR-03 p95 < 1000ms`: 846.91ms - виконується
- `SCN-04 errors < 1%`: 0.00% - виконується
- `vus_max >= 500`: 500 - виконується

Висновок: NFR-03 та NFR-15 формально виконуються: система витримала 500 VUs, p95 для course feed залишився нижче 1000ms, помилок HTTP не було. Водночас зафіксовано хвости latency: 1.72% check-ів на час відповіді > 1000ms не пройшли, максимальний час HTTP-запиту досяг 11.76s.

### RES-05: Stress - Course Creation

Дата/час: 2026-05-12  
Тривалість: 6 хв 00.6 сек  
Max VUs: 100  
Запитів: 47 097  
RPS: 130.60 avg  
p50: 9.46ms  
p95: 109.67ms; Course_Create p95 = 110.17ms  
p99: не виведено у стандартному k6 summary  
Error rate: 0.00%  
Thresholds:

- `checks rate > 98%`: 100.00% - виконується
- `Course_Create p95 < 500ms`: 110.17ms - виконується
- `SCN-05 errors < 1%`: 0.00% - виконується

Висновок: NFR-04 виконується при stress-навантаженні до 100 VUs. Course creation має значний запас: p95 = 110.17ms при цілі <= 500ms. Помилок і порушень checks не зафіксовано.

## Matrix: Scenarios - NFR - Result

| Сценарій | NFR | Threshold | Результат | Статус |
|---|---|---|---|---|
| SCN-01 Smoke | NFR-01 Login Latency | p95 <= 500ms | p95 = 62.07ms | + |
| SCN-01 Smoke | NFR-09 Availability | error rate < 1% | 0.00% | + |
| SCN-02 Login Load | NFR-01 Login Latency | p95 <= 500ms | p95 = 112.88ms | + |
| SCN-02 Login Load | NFR-09 Availability | error rate < 0.5% | 0.00% | + |
| SCN-03 Course List | NFR-02 Course List Loading | p95 <= 800ms | p95 = 5.36ms | + |
| SCN-03 Course List | NFR-09 Availability | error rate < 0.5% | 0.00% | + |
| SCN-04 Course Feed | NFR-03 Course Feed Loading | p95 <= 1000ms | p95 = 846.91ms | + |
| SCN-04 Course Feed | NFR-15 Concurrent Users | vus_max >= 500 | vus_max = 500 | + |
| SCN-04 Course Feed | NFR-09 Availability | error rate < 1% | 0.00% | + |
| SCN-05 Course Creation | NFR-04 Course Creation | p95 <= 500ms | p95 = 110.17ms | + |
| SCN-05 Course Creation | NFR-10 Database Consistency | errors < 1% | 0.00% | + |

## Bottleneck Analysis

### BOT-01:

Виявлено в: SCN-04 Course Feed Loading, при навантаженні до 500 VUs  
Симптом: p95 = 846.91ms виконує NFR-03, але 1.72% latency checks перевищили 1000ms, максимальний HTTP-запит тривав 11.76s.  
Гіпотеза: при 500 concurrent users окремі read-запити до course/module/lesson починають чекати на ресурси БД або thread pool; також один user-flow виконує 5 послідовних HTTP-запитів, що збільшує iteration duration.  
Докази: `checks_failed = 15 663`, `http_req_duration max = 11.76s`, `iteration_duration p95 = 5.14s`, при цьому HTTP error rate = 0.00%.  
Рекомендація: додати моніторинг БД під час spike, перевірити індекси для course/module/lesson queries, розглянути кешування course feed або об'єднання кількох read-запитів в один endpoint для сторінки курсу.

### BOT-02:

Виявлено в: SCN-05 Course Creation Stress, при навантаженні до 100 VUs  
Симптом: NFR-04 виконується, але для write-flow зафіксовано хвости latency: загальний max для HTTP-запитів = 594.84ms, тоді як p95 лишається низьким.  
Гіпотеза: поодинокі затримки можуть виникати під час одночасного створення course/module/lesson через роботу БД, індекси, генерацію позицій або транзакційні блокування.  
Докази: `Course_Create p95 = 110.17ms`, `http_req_duration p95 = 109.67ms`, `http_req_duration max = 594.84ms`, `http_req_failed = 0.00%`, `checks = 100.00%`.  
Рекомендація: під час наступних stress-запусків додати збір DB metrics, перевірити повільні SQL-запити, індекси та блокування.

## Conclusion

За результатами запусків NFR-01, NFR-02, NFR-03, NFR-04, NFR-09, NFR-10 та NFR-15 виконуються: login latency, course list loading, course feed loading і course creation залишаються в межах заданих p95-thresholds, HTTP error rate у всіх сценаріях дорівнює 0.00%, а SCN-04 підтвердив підтримку 500 concurrent users. Критичним ризиком є tail latency у SCN-04: хоча p95 проходить NFR-03, частина запитів перевищує 1000ms, а максимум досягає 11.76s. NFR-05 не перевірявся, оскільки Assignment API наразі не реалізований.
