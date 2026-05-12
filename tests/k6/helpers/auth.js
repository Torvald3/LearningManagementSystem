import http from 'k6/http';
import { check } from 'k6';
import { apiUrl, DEFAULT_PASSWORD, jsonHeaders, requestParams, uniqueSuffix } from './config.js';
import { pick, safeJson } from './checks.js';

export function loginUser(email, password, metricName = 'Login', extraTags = {}) {
  const response = http.post(
    apiUrl('/api/auth/login'),
    JSON.stringify({ email, password }),
    requestParams(metricName, null, {
      endpoint: '/api/auth/login',
      ...extraTags,
    }),
  );

  const body = safeJson(response);
  const accessToken = pick(body, 'accessToken', 'AccessToken');

  check(response, {
    [`${metricName}: status is 200`]: (r) => r.status === 200,
    [`${metricName}: access token returned`]: () => Boolean(accessToken),
  });

  return {
    response,
    body,
    accessToken,
  };
}

export function createConfirmedUser(label = 'user') {
  const providedEmail = __ENV.K6_TEST_EMAIL;
  const suffix = uniqueSuffix(label);
  const email = providedEmail || `k6.${suffix}@example.com`;
  const password = DEFAULT_PASSWORD;

  if (!providedEmail) {
    const registerResponse = http.post(
      apiUrl('/api/auth/register'),
      JSON.stringify({
        email,
        password,
        username: `k6-${suffix}`,
      }),
      requestParams('Setup_Register', null, {
        endpoint: '/api/auth/register',
        phase: 'setup',
      }),
    );

    const registerBody = safeJson(registerResponse);
    const userId = pick(registerBody, 'userId', 'UserId');
    const confirmationToken = pick(registerBody, 'confirmationToken', 'ConfirmationToken');

    check(registerResponse, {
      'setup register: status is 200': (r) => r.status === 200,
      'setup register: user id returned': () => Boolean(userId),
      'setup register: confirmation token returned': () => Boolean(confirmationToken),
    });

    if (!userId || !confirmationToken) {
      throw new Error(`Could not register k6 test user. Status: ${registerResponse.status}`);
    }

    const confirmResponse = http.post(
      apiUrl('/api/auth/confirm-email'),
      JSON.stringify({
        userId,
        token: confirmationToken,
      }),
      {
        headers: jsonHeaders(),
        tags: {
          name: 'Setup_Confirm_Email',
          endpoint: '/api/auth/confirm-email',
          phase: 'setup',
        },
      },
    );

    check(confirmResponse, {
      'setup confirm email: status is 204': (r) => r.status === 204,
    });

    if (confirmResponse.status !== 204) {
      throw new Error(`Could not confirm k6 test user email. Status: ${confirmResponse.status}`);
    }
  }

  const login = loginUser(email, password, 'Setup_Login', {
    phase: 'setup',
  });

  if (!login.accessToken) {
    throw new Error(`Could not log in k6 test user. Status: ${login.response.status}`);
  }

  return {
    email,
    password,
    token: login.accessToken,
  };
}
