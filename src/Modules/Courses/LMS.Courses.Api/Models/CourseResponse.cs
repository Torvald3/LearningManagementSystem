namespace LMS.Courses.Api.Models;

public record CourseResponse(
    Guid Id,
    Guid AuthorId,
    string Title,
    string Theme,
    string Description,
    DateTime CreatedAt,
    DateTime UpdatedAt);