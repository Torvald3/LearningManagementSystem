namespace LMS.Courses.Application.Models;

public record LessonSummary(
    Guid Id,
    Guid ModuleId,
    string Title,
    int Position);
