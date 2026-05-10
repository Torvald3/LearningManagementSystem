using LMS.Common.CQRS;

namespace LMS.Courses.Application.Commands.ArchiveCourseModule;

public record ArchiveCourseModuleCommand(
    Guid CourseId,
    Guid ModuleId) : ICommand;
