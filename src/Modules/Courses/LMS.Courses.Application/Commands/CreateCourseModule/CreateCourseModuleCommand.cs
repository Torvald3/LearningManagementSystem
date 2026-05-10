using LMS.Common.CQRS;

namespace LMS.Courses.Application.Commands.CreateCourseModule;

public record CreateCourseModuleCommand(
    Guid CourseId,
    string Title,
    string Description) : ICommand;
