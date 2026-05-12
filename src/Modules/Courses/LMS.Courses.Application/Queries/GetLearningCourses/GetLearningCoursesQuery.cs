using LMS.Common.CQRS;

namespace LMS.Courses.Application.Queries.GetLearningCourses;

public record GetLearningCoursesQuery(Guid UserId) : IQuery;
