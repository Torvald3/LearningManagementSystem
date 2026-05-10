namespace LMS.Courses.Infrastructure.Entities;

public class CourseModule
{
    public Guid Id { get; set; }

    public Guid CourseId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int Position { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool IsArchived { get; set; }

    public DateTime? ArchivedAt { get; set; }

    public Course Course { get; set; } = null!;

    public ICollection<Lesson> Lessons { get; set; } = [];
}
