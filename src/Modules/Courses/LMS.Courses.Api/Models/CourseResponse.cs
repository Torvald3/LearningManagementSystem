namespace LMS.Courses.Api.Models;

public class CourseResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Theme { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}