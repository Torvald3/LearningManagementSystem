namespace LMS.Courses.Application.Models;

public record CourseModuleSummary(
    Guid Id,
    Guid CourseId,
    string Title,
    string Description,
    int Position,
    int LessonsCount);
