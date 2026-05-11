namespace LMS.Courses.Api.Models;

public record UpdateCourseModuleRequest(
    string Title,
    string Description,
    int Position);
