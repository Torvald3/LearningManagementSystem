namespace LMS.Courses.Api.Models;

public class CreateCourseRequest
{
    public string Title { get; set; } = null!;
    public string Theme { get; set; } = null!;
    public string Description { get; set; } = null!;
}