import http from 'k6/http';
import { check, sleep } from 'k6';
import { Counter, Trend as Histogram } from 'k6/metrics';
import { apiUrl, DEFAULT_SLEEP_SECONDS, requestParams } from '../helpers/config.js';
import { createConfirmedUser } from '../helpers/auth.js';
import { createCourseFixture } from '../helpers/course-data.js';
import { recordResponse } from '../helpers/checks.js';

export const options = {
  stages: [
    { duration: '1m', target: 100 },
    { duration: '3m', target: 500 },
    { duration: '5m', target: 500 },
    { duration: '1m', target: 0 },
  ],
  thresholds: {
    'http_req_duration{nfr:NFR-03}': ['p(95)<1000'],
    'http_req_failed{scenario:SCN-04}': ['rate<0.01'],
    checks: ['rate>0.98'],
    vus_max: ['value>=500'],
  },
};

const courseFeedRequests = new Counter('course_feed_requests_total');
const courseFeedDuration = new Histogram('course_feed_duration_ms');

export function setup() {
  const user = createConfirmedUser('course-feed');
  const fixture = createCourseFixture(user.token, 'course-feed');

  return {
    user,
    fixture,
  };
}

export default function (data) {
  const { courseId, moduleId, lessonId } = data.fixture;
  const commonTags = {
    scenario: 'SCN-04',
    nfr: 'NFR-03',
  };

  const course = http.get(
    apiUrl(`/api/courses/${courseId}`),
    requestParams('Course_Feed_Course', data.user.token, {
      ...commonTags,
      endpoint: '/api/courses/{courseId}',
    }),
  );
  recordResponse(courseFeedRequests, courseFeedDuration, course);
  check(course, {
    'course feed course: status is 200': (r) => r.status === 200,
    'course feed course: response under 1000ms': (r) => r.timings.duration <= 1000,
  });

  const modules = http.get(
    apiUrl(`/api/courses/${courseId}/modules`),
    requestParams('Course_Feed_Modules', data.user.token, {
      ...commonTags,
      endpoint: '/api/courses/{courseId}/modules',
    }),
  );
  recordResponse(courseFeedRequests, courseFeedDuration, modules);
  check(modules, {
    'course feed modules: status is 200': (r) => r.status === 200,
    'course feed modules: response under 1000ms': (r) => r.timings.duration <= 1000,
  });

  const module = http.get(
    apiUrl(`/api/courses/${courseId}/modules/${moduleId}`),
    requestParams('Course_Feed_Module', data.user.token, {
      ...commonTags,
      endpoint: '/api/courses/{courseId}/modules/{moduleId}',
    }),
  );
  recordResponse(courseFeedRequests, courseFeedDuration, module);
  check(module, {
    'course feed module: status is 200': (r) => r.status === 200,
    'course feed module: response under 1000ms': (r) => r.timings.duration <= 1000,
  });

  const lessons = http.get(
    apiUrl(`/api/courses/${courseId}/modules/${moduleId}/lessons`),
    requestParams('Course_Feed_Lessons', data.user.token, {
      ...commonTags,
      endpoint: '/api/courses/{courseId}/modules/{moduleId}/lessons',
    }),
  );
  recordResponse(courseFeedRequests, courseFeedDuration, lessons);
  check(lessons, {
    'course feed lessons: status is 200': (r) => r.status === 200,
    'course feed lessons: response under 1000ms': (r) => r.timings.duration <= 1000,
  });

  const lesson = http.get(
    apiUrl(`/api/courses/${courseId}/modules/${moduleId}/lessons/${lessonId}`),
    requestParams('Course_Feed_Lesson', data.user.token, {
      ...commonTags,
      endpoint: '/api/courses/{courseId}/modules/{moduleId}/lessons/{lessonId}',
    }),
  );
  recordResponse(courseFeedRequests, courseFeedDuration, lesson);
  check(lesson, {
    'course feed lesson: status is 200': (r) => r.status === 200,
    'course feed lesson: response under 1000ms': (r) => r.timings.duration <= 1000,
  });

  sleep(DEFAULT_SLEEP_SECONDS);
}
