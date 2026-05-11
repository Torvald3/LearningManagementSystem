using LMS.Common.CQRS;
using LMS.Common.Results;
using LMS.Courses.Application.Errors;
using LMS.Courses.Application.Models;
using LMS.Courses.Core.Services;
using Microsoft.Extensions.Logging;

namespace LMS.Courses.Application.Commands.UpdateLesson;

public class UpdateLessonCommandHandler : ICommandHandler<UpdateLessonCommand, Lesson>
{
    private readonly ICoursesService _coursesService;
    private readonly ILogger<UpdateLessonCommandHandler> _logger;

    public UpdateLessonCommandHandler(
        ICoursesService coursesService,
        ILogger<UpdateLessonCommandHandler> logger)
    {
        _coursesService = coursesService;
        _logger = logger;
    }

    public async Task<Result<Lesson>> HandleAsync(
        UpdateLessonCommand command,
        CancellationToken cancellationToken = default)
    {
        var course = await _coursesService.GetCourseAsync(command.CourseId, cancellationToken);

        if (course is null)
        {
            _logger.LogWarning(
                "timestamp={Timestamp} level={Level} event={Event} course_id={CourseId}",
                DateTime.UtcNow,
                "WARN",
                "lesson.update.course_not_found",
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
                "lesson.update.module_not_found",
                command.CourseId,
                command.ModuleId);

            return CourseErrors.ModuleNotFound(command.ModuleId);
        }

        var existingLesson = await _coursesService.GetLessonAsync(
            command.ModuleId,
            command.LessonId,
            cancellationToken);

        if (existingLesson is null)
        {
            _logger.LogWarning(
                "timestamp={Timestamp} level={Level} event={Event} module_id={ModuleId} lesson_id={LessonId}",
                DateTime.UtcNow,
                "WARN",
                "lesson.update.lesson_not_found",
                command.ModuleId,
                command.LessonId);

            return CourseErrors.LessonNotFound(command.LessonId);
        }

        var lessonsCount = await _coursesService.GetLessonsCountAsync(command.ModuleId, cancellationToken);

        if (command.Position < 1 || command.Position > lessonsCount)
        {
            return CourseErrors.InvalidPosition(lessonsCount);
        }

        var updatedLesson = new LMS.Courses.Core.Models.Lesson
        {
            Id = existingLesson.Id,
            ModuleId = existingLesson.ModuleId,
            Title = command.Title.Trim(),
            Content = command.Content.Trim(),
            Position = command.Position,
            CreatedAt = existingLesson.CreatedAt,
            UpdatedAt = DateTime.UtcNow
        };

        var updated = await _coursesService.UpdateLessonAsync(updatedLesson, cancellationToken);

        if (!updated)
        {
            return CourseErrors.LessonNotFound(command.LessonId);
        }

        _logger.LogInformation(
            "timestamp={Timestamp} level={Level} event={Event} module_id={ModuleId} lesson_id={LessonId} position={Position}",
            DateTime.UtcNow,
            "INFO",
            "lesson.update.succeeded",
            updatedLesson.ModuleId,
            updatedLesson.Id,
            updatedLesson.Position);

        return new Lesson(
            updatedLesson.Id,
            updatedLesson.ModuleId,
            updatedLesson.Title,
            updatedLesson.Content,
            updatedLesson.Position,
            updatedLesson.CreatedAt,
            updatedLesson.UpdatedAt);
    }
}
