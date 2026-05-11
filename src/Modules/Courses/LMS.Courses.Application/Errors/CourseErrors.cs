using LMS.Common.Results;

namespace LMS.Courses.Application.Errors;

public static class CourseErrors
{
    public static Error AuthorNotFound(Guid authorId) =>
        Error.Validation(
            "courses.author_not_found",
            $"Author with id {authorId} does not exist.");

    public static Error CourseNotFound(Guid courseId) =>
        Error.NotFound(
            "courses.course_not_found",
            $"Course with id {courseId} not found.");

    public static Error ModuleNotFound(Guid moduleId) =>
        Error.NotFound(
            "courses.module_not_found",
            $"Module with id {moduleId} not found.");

    public static Error LessonNotFound(Guid lessonId) =>
        Error.NotFound(
            "courses.lesson_not_found",
            $"Lesson with id {lessonId} not found.");

    public static Error InvalidPosition(int maxPosition) =>
        Error.Validation(
            "courses.invalid_position",
            $"Position must be between 1 and {maxPosition}.");
}
