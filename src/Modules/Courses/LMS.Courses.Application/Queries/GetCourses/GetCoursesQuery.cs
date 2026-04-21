using LMS.Common.CQRS;

namespace LMS.Courses.Application.Queries.GetCourses;

// pagination and filtration parameter can be there
public record GetCoursesQuery() : IQuery;