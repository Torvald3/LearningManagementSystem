import http from 'k6/http';
import { check, sleep } from 'k6';
import { Counter, Trend as Histogram } from 'k6/metrics';
import { apiUrl, DEFAULT_SLEEP_SECONDS, requestParams } from '../helpers/config.js';
import { createConfirmedUser, loginUser } from '../helpers/auth.js';
import { checkStatus, recordResponse } from '../helpers/checks.js';

export const options = {
  stages: [
    { duration: '30s', target: 1 },
  ],
  thresholds: {
    'http_req_duration{name:Smoke_Login}': ['p(95)<500'],
    http_req_failed: ['rate<0.01'],
    checks: ['rate>0.99'],
  },
};

const smokeRequests = new Counter('smoke_requests_total');
const smokeDuration = new Histogram('smoke_request_duration_ms');

export function setup() {
  return {
    user: createConfirmedUser('smoke'),
  };
}

export default function (data) {
  const login = loginUser(data.user.email, data.user.password, 'Smoke_Login', {
    scenario: 'SCN-01',
    nfr: 'NFR-01',
  });
  recordResponse(smokeRequests, smokeDuration, login.response);

  const token = login.accessToken || data.user.token;

  const authTest = http.get(
    apiUrl('/api/auth/test'),
    requestParams('Smoke_Auth_Test', token, {
      scenario: 'SCN-01',
      nfr: 'NFR-09',
      endpoint: '/api/auth/test',
    }),
  );
  recordResponse(smokeRequests, smokeDuration, authTest);
  checkStatus(authTest, 200, 'smoke auth test');

  const courses = http.get(
    apiUrl('/api/courses'),
    requestParams('Smoke_Courses_List', token, {
      scenario: 'SCN-01',
      nfr: 'NFR-09',
      endpoint: '/api/courses',
    }),
  );
  recordResponse(smokeRequests, smokeDuration, courses);
  check(courses, {
    'smoke courses: status is 200': (r) => r.status === 200,
    'smoke courses: body is json': (r) => {
      try {
        r.json();
        return true;
      } catch (_) {
        return false;
      }
    },
  });

  const metrics = http.get(
    apiUrl('/metrics'),
    requestParams('Smoke_Metrics', null, {
      scenario: 'SCN-01',
      nfr: 'NFR-09',
      endpoint: '/metrics',
    }),
  );
  recordResponse(smokeRequests, smokeDuration, metrics);
  check(metrics, {
    'smoke metrics: status is 200': (r) => r.status === 200,
  });

  sleep(DEFAULT_SLEEP_SECONDS);
}
