import http from 'k6/http';
import { check, sleep } from 'k6';
import { Counter, Trend as Histogram } from 'k6/metrics';
import { DEFAULT_SLEEP_SECONDS } from '../helpers/config.js';
import { createConfirmedUser, loginUser } from '../helpers/auth.js';
import { recordResponse } from '../helpers/checks.js';

export const options = {
  stages: [
    { duration: '1m', target: 50 },
    { duration: '3m', target: 50 },
    { duration: '1m', target: 0 },
  ],
  thresholds: {
    'http_req_duration{name:Login_Load}': ['p(95)<500'],
    'http_req_failed{name:Login_Load}': ['rate<0.005'],
    checks: ['rate>0.99'],
  },
};

const loginAttempts = new Counter('login_attempts_total');
const loginDuration = new Histogram('login_duration_ms');

export function setup() {
  return {
    user: createConfirmedUser('login-load'),
  };
}

export default function (data) {
  const login = loginUser(data.user.email, data.user.password, 'Login_Load', {
    scenario: 'SCN-02',
    nfr: 'NFR-01',
  });

  recordResponse(loginAttempts, loginDuration, login.response);

  check(login.response, {
    'login load: p95 candidate under 500ms': (r) => r.timings.duration <= 500,
  });

  sleep(DEFAULT_SLEEP_SECONDS);
}
