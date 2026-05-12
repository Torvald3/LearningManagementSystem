namespace LMS.Courses.Api.Models;

public record CreateCourseRequest(
    string Title,
    string Theme,
    string Description);
