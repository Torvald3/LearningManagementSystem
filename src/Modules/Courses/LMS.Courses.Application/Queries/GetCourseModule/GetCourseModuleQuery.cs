using LMS.Common.CQRS;

namespace LMS.Courses.Application.Queries.GetCourseModule;

public record GetCourseModuleQuery(
    Guid CourseId,
    Guid ModuleId) : IQuery;
