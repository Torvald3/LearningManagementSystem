using LMS.Courses.Core.Models;

namespace LMS.Courses.Application.Models;

public record CourseMember(
    Guid Id,
    Guid CourseId,
    Guid UserId,
    CourseRole Role,
    DateTime CreatedAt,
    DateTime UpdatedAt);
