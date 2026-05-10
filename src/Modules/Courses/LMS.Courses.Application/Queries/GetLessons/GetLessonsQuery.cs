using LMS.Common.CQRS;

namespace LMS.Courses.Application.Queries.GetLessons;

public record GetLessonsQuery(
    Guid CourseId,
    Guid ModuleId) : IQuery;
