using LMS.Courses.Application.Models;

namespace LMS.Courses.Application.Commands.UpdateCourse;

public record UpdateCourseResult(
    UpdateCourseStatus Status,
    Course? Course,
    IEnumerable<string> Errors);