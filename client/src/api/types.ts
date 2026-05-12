export type LoginRequest = { email: string; password: string }
export type LoginResponse = { accessToken: string; expiresAtUtc: string }

export type RegisterUserRequest = {
  email: string
  password: string
  username: string
}
export type RegisterUserResponse = {
  userId: string
  email: string
  confirmationToken: string | null
}

export type ConfirmEmailRequest = { userId: string; token: string }

export type CourseResponse = {
  id: string
  authorId: string
  title: string
  theme: string
  description: string
  createdAt: string
  updatedAt: string
}

export type CreateCourseRequest = {
  authorId: string
  title: string
  theme: string
  description: string
}

export type CourseModuleSummaryResponse = {
  id: string
  courseId: string
  title: string
  description: string
  position: number
  lessonsCount: number
}

export type CourseModuleResponse = {
  id: string
  courseId: string
  title: string
  description: string
  position: number
  createdAt: string
  updatedAt: string
}

export type LessonSummaryResponse = {
  id: string
  moduleId: string
  title: string
  position: number
}

export type LessonResponse = {
  id: string
  moduleId: string
  title: string
  content: string
  position: number
  createdAt: string
  updatedAt: string
}

export type CreateCourseModuleRequest = { title: string; description: string }
export type CreateLessonRequest = { title: string; content: string }
