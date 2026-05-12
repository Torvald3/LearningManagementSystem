import http from 'k6/http';
import { check, sleep } from 'k6';
import { Counter, Trend as Histogram } from 'k6/metrics';
import { apiUrl, DEFAULT_SLEEP_SECONDS, requestParams } from '../helpers/config.js';
import { createConfirmedUser } from '../helpers/auth.js';
import { createCourseFixture } from '../helpers/course-data.js';
import { recordResponse } from '../helpers/checks.js';

export const options = {
  stages: [
    { duration: '2m', target: 100 },
    { duration: '5m', target: 100 },
    { duration: '1m', target: 0 },
  ],
  thresholds: {
    'http_req_duration{nfr:NFR-02}': ['p(95)<800'],
    'http_req_failed{nfr:NFR-02}': ['rate<0.005'],
    checks: ['rate>0.99'],
  },
};

const courseListRequests = new Counter('course_list_requests_total');
const courseListDuration = new Histogram('course_list_duration_ms');

export function setup() {
  const user = createConfirmedUser('course-list');
  const fixture = createCourseFixture(user.token, 'course-list');

  return {
    user,
    fixture,
  };
}

export default function (data) {
  const commonTags = {
    scenario: 'SCN-03',
    nfr: 'NFR-02',
  };

  const allCourses = http.get(
    apiUrl('/api/courses'),
    requestParams('Course_List_All', data.user.token, {
      ...commonTags,
      endpoint: '/api/courses',
    }),
  );
  recordResponse(courseListRequests, courseListDuration, allCourses);
  check(allCourses, {
    'course list all: status is 200': (r) => r.status === 200,
    'course list all: response under 800ms': (r) => r.timings.duration <= 800,
  });

  const learningCourses = http.get(
    apiUrl('/api/courses/my/learning'),
    requestParams('Course_List_Learning', data.user.token, {
      ...commonTags,
      endpoint: '/api/courses/my/learning',
    }),
  );
  recordResponse(courseListRequests, courseListDuration, learningCourses);
  check(learningCourses, {
    'course list learning: status is 200': (r) => r.status === 200,
    'course list learning: response under 800ms': (r) => r.timings.duration <= 800,
  });

  const teachingCourses = http.get(
    apiUrl('/api/courses/my/teaching'),
    requestParams('Course_List_Teaching', data.user.token, {
      ...commonTags,
      endpoint: '/api/courses/my/teaching',
    }),
  );
  recordResponse(courseListRequests, courseListDuration, teachingCourses);
  check(teachingCourses, {
    'course list teaching: status is 200': (r) => r.status === 200,
    'course list teaching: response under 800ms': (r) => r.timings.duration <= 800,
  });

  sleep(DEFAULT_SLEEP_SECONDS);
}
