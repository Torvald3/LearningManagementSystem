namespace LMS.Courses.Core.Models;

public class Course
{
    public Guid Id { get; set; }
    
    public Guid AuthorId { get; set; }
    
    public string Title { get; set; } = string.Empty;
    
    public string Theme { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
}