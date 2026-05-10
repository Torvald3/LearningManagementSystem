namespace LMS.Courses.Api.Models;

public record LessonSummaryResponse(
    Guid Id,
    Guid ModuleId,
    string Title,
    int Position);
