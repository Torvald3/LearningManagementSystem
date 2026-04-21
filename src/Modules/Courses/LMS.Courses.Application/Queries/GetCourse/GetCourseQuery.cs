using LMS.Common.CQRS;

namespace LMS.Courses.Application.Queries.GetCourse;

public record GetCourseQuery(Guid CourseId) : IQuery;