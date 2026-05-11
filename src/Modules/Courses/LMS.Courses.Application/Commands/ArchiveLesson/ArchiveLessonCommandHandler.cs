using LMS.Common.CQRS;
using LMS.Common.Results;
using LMS.Courses.Application.Errors;
using LMS.Courses.Core.Services;
using Microsoft.Extensions.Logging;

namespace LMS.Courses.Application.Commands.ArchiveLesson;

public class ArchiveLessonCommandHandler : ICommandHandler<ArchiveLessonCommand>
{
    private readonly ICoursesService _coursesService;
    private readonly ILogger<ArchiveLessonCommandHandler> _logger;

    public ArchiveLessonCommandHandler(
        ICoursesService coursesService,
        ILogger<ArchiveLessonCommandHandler> logger)
    {
        _coursesService = coursesService;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(
        ArchiveLessonCommand command,
        CancellationToken cancellationToken = default)
    {
        var course = await _coursesService.GetCourseAsync(command.CourseId, cancellationToken);

        if (course is null)
        {
            _logger.LogWarning(
                "timestamp={Timestamp} level={Level} event={Event} course_id={CourseId}",
                DateTime.UtcNow,
                "WARN",
                "lesson.archive.course_not_found",
                command.CourseId);

            return CourseErrors.CourseNotFound(command.CourseId);
        }

        var module = await _coursesService.GetCourseModuleAsync(
            command.CourseId,
            command.ModuleId,
            cancellationToken);

        if (module is null)
        {
            _logger.LogWarning(
                "timestamp={Timestamp} level={Level} event={Event} course_id={CourseId} module_id={ModuleId}",
                DateTime.UtcNow,
                "WARN",
                "lesson.archive.module_not_found",
                command.CourseId,
                command.ModuleId);

            return CourseErrors.ModuleNotFound(command.ModuleId);
        }

        var lesson = await _coursesService.GetLessonAsync(
            command.ModuleId,
            command.LessonId,
            cancellationToken);

        if (lesson is null)
        {
            _logger.LogWarning(
                "timestamp={Timestamp} level={Level} event={Event} module_id={ModuleId} lesson_id={LessonId}",
                DateTime.UtcNow,
                "WARN",
                "lesson.archive.lesson_not_found",
                command.ModuleId,
                command.LessonId);

            return CourseErrors.LessonNotFound(command.LessonId);
        }

        var archived = await _coursesService.ArchiveLessonAsync(
            command.ModuleId,
            command.LessonId,
            DateTime.UtcNow,
            cancellationToken);

        if (!archived)
        {
            return CourseErrors.LessonNotFound(command.LessonId);
        }

        _logger.LogInformation(
            "timestamp={Timestamp} level={Level} event={Event} module_id={ModuleId} lesson_id={LessonId}",
            DateTime.UtcNow,
            "INFO",
            "lesson.archive.succeeded",
            command.ModuleId,
            command.LessonId);

        return Result.Success;
    }
}
