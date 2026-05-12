using LMS.Courses.Core.Models;

namespace LMS.Courses.Infrastructure.Entities;

public class CourseMember
{
    public Guid Id { get; set; }

    public Guid CourseId { get; set; }

    public Guid UserId { get; set; }

    public CourseRole Role { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public Course Course { get; set; } = null!;
}
