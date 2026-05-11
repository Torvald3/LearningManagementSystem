using LMS.Courses.Application.Models;

namespace LMS.Courses.Application.Commands.UpdateCourseModule;

public record UpdateCourseModuleResult(
    UpdateCourseModuleStatus Status,
    CourseModule? Module,
    IEnumerable<string> Errors);
