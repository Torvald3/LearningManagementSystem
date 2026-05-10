namespace LMS.Courses.Application.Models;

public record CourseModule(
    Guid Id,
    Guid CourseId,
    string Title,
    string Description,
    int Position,
    DateTime CreatedAt,
    DateTime UpdatedAt);
