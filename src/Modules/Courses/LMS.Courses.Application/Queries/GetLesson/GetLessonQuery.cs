using LMS.Common.CQRS;

namespace LMS.Courses.Application.Queries.GetLesson;

public record GetLessonQuery(
    Guid CourseId,
    Guid ModuleId,
    Guid LessonId) : IQuery;
