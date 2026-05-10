using LMS.Common.CQRS;

namespace LMS.Courses.Application.Commands.UpdateCourseModule;

public record UpdateCourseModuleCommand(
    Guid CourseId,
    Guid ModuleId,
    string Title,
    string Description,
    int Position) : ICommand;
