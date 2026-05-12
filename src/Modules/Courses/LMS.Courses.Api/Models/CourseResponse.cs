namespace LMS.Courses.Api.Models;

public record CourseResponse(
    Guid Id,
    string Title,
    string Theme,
    string Description,
    DateTime CreatedAt,
    DateTime UpdatedAt);
