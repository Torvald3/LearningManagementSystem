namespace LMS.Courses.Application.Commands.ArchiveLesson;

public record ArchiveLessonResult(
    ArchiveLessonStatus Status,
    IEnumerable<string> Errors);
