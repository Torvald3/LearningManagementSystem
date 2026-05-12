using LMS.Common.CQRS;

namespace LMS.Courses.Application.Queries.GetTeachingCourses;

public record GetTeachingCoursesQuery(Guid UserId) : IQuery;
