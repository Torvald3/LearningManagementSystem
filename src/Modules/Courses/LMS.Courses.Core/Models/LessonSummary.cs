namespace LMS.Courses.Core.Models;

public class LessonSummary
{
    public Guid Id { get; set; }

    public Guid ModuleId { get; set; }

    public string Title { get; set; } = string.Empty;

    public int Position { get; set; }
}
