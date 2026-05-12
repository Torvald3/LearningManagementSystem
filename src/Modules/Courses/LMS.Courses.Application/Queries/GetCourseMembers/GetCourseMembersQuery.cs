using LMS.Common.CQRS;

namespace LMS.Courses.Application.Queries.GetCourseMembers;

public record GetCourseMembersQuery(Guid CourseId) : IQuery;
