import { check } from 'k6';

export function safeJson(response) {
  try {
    return response.json();
  } catch (_) {
    return null;
  }
}

export function pick(body, camelCaseName, pascalCaseName = null) {
  if (!body) {
    return undefined;
  }

  return body[camelCaseName] ?? body[pascalCaseName ?? camelCaseName];
}

export function checkStatus(response, expectedStatus, label) {
  return check(response, {
    [`${label}: status is ${expectedStatus}`]: (r) => r.status === expectedStatus,
  });
}

export function checkSuccessful(response, label) {
  return check(response, {
    [`${label}: status is 2xx`]: (r) => r.status >= 200 && r.status < 300,
  });
}

export function recordResponse(counter, histogram, response) {
  counter.add(1);
  histogram.add(response.timings.duration);
}
