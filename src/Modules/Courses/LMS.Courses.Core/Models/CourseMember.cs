namespace LMS.Courses.Core.Models;

public class CourseMember
{
    public Guid Id { get; set; }

    public Guid CourseId { get; set; }

    public Guid UserId { get; set; }

    public CourseRole Role { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
