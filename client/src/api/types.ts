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
  title: string
  theme: string
  description: string
  createdAt: string
  updatedAt: string
}

export type CreateCourseRequest = {
  title: string
  theme: string
  description: string
}

export type MediaEntityType = 'Course' | 'CourseModule' | 'Lesson' | 'User'

export type MediaResponse = {
  id: string
  entityType: MediaEntityType
  entityId: string
  originalFileName: string
  contentType: string
  size: number
  createdAt: string
}

export type MediaUrlResponse = {
  url: string
  expiresAt: string
}

export type UserResponse = {
  id: string
  username: string
  email: string
  bio: string | null
  avatarMediaId: string | null
}

export type CourseMemberResponse = {
  id: string
  courseId: string
  userId: string
  role: string
  createdAt: string
  updatedAt: string
}

export type AddCourseMemberRequest = {
  userId: string
  role: string
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
export type UpdateCourseModuleRequest = { title: string; description: string; position: number }
export type UpdateLessonRequest = { title: string; content: string; position: number }
