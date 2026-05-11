namespace LMS.Courses.Core.Models;

public class CourseModuleSummary
{
    public Guid Id { get; set; }

    public Guid CourseId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int Position { get; set; }

    public int LessonsCount { get; set; }
}
