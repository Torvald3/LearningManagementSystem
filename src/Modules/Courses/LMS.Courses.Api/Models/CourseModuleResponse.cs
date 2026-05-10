namespace LMS.Courses.Api.Models;

public record CourseModuleResponse(
    Guid Id,
    Guid CourseId,
    string Title,
    string Description,
    int Position,
    DateTime CreatedAt,
    DateTime UpdatedAt);
