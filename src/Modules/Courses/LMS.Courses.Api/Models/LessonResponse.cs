namespace LMS.Courses.Api.Models;

public record LessonResponse(
    Guid Id,
    Guid ModuleId,
    string Title,
    string Content,
    int Position,
    DateTime CreatedAt,
    DateTime UpdatedAt);
