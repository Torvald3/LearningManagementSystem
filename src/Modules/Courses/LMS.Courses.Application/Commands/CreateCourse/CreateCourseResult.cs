using LMS.Courses.Application.Models;

namespace LMS.Courses.Application.Commands.CreateCourse;

public record CreateCourseResult(
    CreateCourseStatus Status,
    Course? Course,
    IEnumerable<string> Errors);