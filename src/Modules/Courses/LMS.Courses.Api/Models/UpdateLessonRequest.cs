namespace LMS.Courses.Api.Models;

public record UpdateLessonRequest(
    string Title,
    string Content,
    int Position);
