namespace LMS.Courses.Api.Models;

public record CourseMemberResponse(
    Guid Id,
    Guid CourseId,
    Guid UserId,
    string Role,
    DateTime CreatedAt,
    DateTime UpdatedAt);
