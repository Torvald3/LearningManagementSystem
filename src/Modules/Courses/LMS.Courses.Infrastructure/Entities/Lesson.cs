namespace LMS.Courses.Infrastructure.Entities;

public class Lesson
{
    public Guid Id { get; set; }

    public Guid ModuleId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public int Position { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool IsArchived { get; set; }

    public DateTime? ArchivedAt { get; set; }

    public CourseModule Module { get; set; } = null!;
}
