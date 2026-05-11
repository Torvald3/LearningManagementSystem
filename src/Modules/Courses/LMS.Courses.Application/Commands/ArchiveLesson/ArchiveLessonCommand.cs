using LMS.Common.CQRS;

namespace LMS.Courses.Application.Commands.ArchiveLesson;

public record ArchiveLessonCommand(
    Guid CourseId,
    Guid ModuleId,
    Guid LessonId) : ICommand;
