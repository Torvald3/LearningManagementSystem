using LMS.Courses.Application.Models;

namespace LMS.Courses.Application.Commands.CreateCourseModule;

public record CreateCourseModuleResult(
    CreateCourseModuleStatus Status,
    CourseModule? Module,
    IEnumerable<string> Errors);
