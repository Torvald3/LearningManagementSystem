namespace LMS.Courses.Api.Models;

public record CreateCourseRequest(
    Guid AuthorId,
    string Title,
    string Theme,
    string Description);