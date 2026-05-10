namespace LMS.Courses.Api.Models;

public record CourseModuleSummaryResponse(
    Guid Id,
    Guid CourseId,
    string Title,
    string Description,
    int Position,
    int LessonsCount);
