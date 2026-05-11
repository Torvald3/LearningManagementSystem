namespace LMS.Courses.Application.Models;

public record Course(
    Guid Id,
    Guid AuthorId,
    string Title,
    string Theme,
    string Description,
    DateTime CreatedAt,
    DateTime UpdatedAt);