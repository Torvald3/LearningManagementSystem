using LMS.Common.CQRS;

namespace LMS.Courses.Application.Queries.GetCourseModules;

public record GetCourseModulesQuery(Guid CourseId) : IQuery;
