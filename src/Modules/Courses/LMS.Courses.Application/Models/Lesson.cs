namespace LMS.Courses.Application.Models;

public record Lesson(
    Guid Id,
    Guid ModuleId,
    string Title,
    string Content,
    int Position,
    DateTime CreatedAt,
    DateTime UpdatedAt);
