namespace LMS.Courses.Api.Models;

public record UpdateCourseRequest(
    string Title, 
    string Theme,
    string Description);