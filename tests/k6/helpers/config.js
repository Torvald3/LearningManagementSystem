export const BASE_URL = __ENV.BASE_URL || __ENV.K6_BASE_URL || 'http://localhost:5277';
export const DEFAULT_PASSWORD = __ENV.K6_TEST_PASSWORD || 'K6Password!2026';
export const DEFAULT_SLEEP_SECONDS = Number(__ENV.K6_SLEEP_SECONDS || '1');

export function apiUrl(path) {
  const normalizedPath = path.startsWith('/') ? path : `/${path}`;

  return `${BASE_URL}${normalizedPath}`;
}

export function jsonHeaders(token) {
  const headers = {
    'Content-Type': 'application/json',
  };

  if (token) {
    headers.Authorization = `Bearer ${token}`;
  }

  return headers;
}

export function requestParams(name, token, extraTags = {}) {
  return {
    headers: jsonHeaders(token),
    tags: {
      name,
      ...extraTags,
    },
  };
}

export function uniqueSuffix(label) {
  return `${label}.${Date.now()}.${Math.floor(Math.random() * 1000000)}`;
}
