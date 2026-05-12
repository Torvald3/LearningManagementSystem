import http from 'k6/http';
import { check, sleep } from 'k6';
import { Counter, Trend as Histogram } from 'k6/metrics';
import { DEFAULT_SLEEP_SECONDS } from '../helpers/config.js';
import { createConfirmedUser } from '../helpers/auth.js';
import { createCourse, createCourseModule, createLesson } from '../helpers/course-data.js';
import { recordResponse } from '../helpers/checks.js';

export const options = {
  stages: [
    { duration: '1m', target: 20 },
    { duration: '2m', target: 50 },
    { duration: '2m', target: 100 },
    { duration: '1m', target: 0 },
  ],
  thresholds: {
    'http_req_duration{name:Course_Create}': ['p(95)<500'],
    'http_req_failed{scenario:SCN-05}': ['rate<0.01'],
    checks: ['rate>0.98'],
  },
};

const courseCreationRequests = new Counter('course_creation_requests_total');
const courseCreationDuration = new Histogram('course_creation_duration_ms');

export function setup() {
  return {
    user: createConfirmedUser('course-create'),
  };
}

export default function (data) {
  const label = `vu-${__VU}-iter-${__ITER}`;
  const commonTags = {
    scenario: 'SCN-05',
    nfr: 'NFR-04',
  };

  const course = createCourse(data.user.token, label, 'Course_Create', commonTags);
  recordResponse(courseCreationRequests, courseCreationDuration, course.response);
  check(course.response, {
    'course creation: response under 500ms': (r) => r.timings.duration <= 500,
  });

  if (!course.courseId) {
    sleep(DEFAULT_SLEEP_SECONDS);
    return;
  }

  const module = createCourseModule(data.user.token, course.courseId, label, 'Course_Module_Create', commonTags);
  recordResponse(courseCreationRequests, courseCreationDuration, module.response);

  if (module.moduleId) {
    const lesson = createLesson(data.user.token, course.courseId, module.moduleId, label, 'Course_Lesson_Create', commonTags);
    recordResponse(courseCreationRequests, courseCreationDuration, lesson.response);
  }

  sleep(DEFAULT_SLEEP_SECONDS);
}
