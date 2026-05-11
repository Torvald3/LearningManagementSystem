namespace LMS.Courses.Application.Commands.ArchiveCourseModule;

public record ArchiveCourseModuleResult(
    ArchiveCourseModuleStatus Status,
    IEnumerable<string> Errors);
