using LMS.Common.CQRS;
using LMS.Common.Results;
using LMS.Courses.Application.Errors;
using LMS.Courses.Application.Models;
using LMS.Courses.Core.Services;
using Microsoft.Extensions.Logging;

namespace LMS.Courses.Application.Commands.CreateLesson;

public class CreateLessonCommandHandler : ICommandHandler<CreateLessonCommand, Lesson>
{
    private readonly ICoursesService _coursesService;
    private readonly ILogger<CreateLessonCommandHandler> _logger;

    public CreateLessonCommandHandler(
        ICoursesService coursesService,
        ILogger<CreateLessonCommandHandler> logger)
    {
        _coursesService = coursesService;
        _logger = logger;
    }

    public async Task<Result<Lesson>> HandleAsync(
        CreateLessonCommand command,
        CancellationToken cancellationToken = default)
    {
        var course = await _coursesService.GetCourseAsync(command.CourseId, cancellationToken);

        if (course is null)
        {
            _logger.LogWarning(
                "timestamp={Timestamp} level={Level} event={Event} course_id={CourseId}",
                DateTime.UtcNow,
                "WARN",
                "lesson.create.course_not_found",
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
                "lesson.create.module_not_found",
                command.CourseId,
                command.ModuleId);

            return CourseErrors.ModuleNotFound(command.ModuleId);
        }

        var now = DateTime.UtcNow;
        var lesson = new LMS.Courses.Core.Models.Lesson
        {
            Id = Guid.NewGuid(),
            ModuleId = command.ModuleId,
            Title = command.Title.Trim(),
            Content = command.Content.Trim(),
            Position = await _coursesService.GetNextLessonPositionAsync(command.ModuleId, cancellationToken),
            CreatedAt = now,
            UpdatedAt = now
        };

        await _coursesService.CreateLessonAsync(lesson, cancellationToken);

        _logger.LogInformation(
            "timestamp={Timestamp} level={Level} event={Event} course_id={CourseId} module_id={ModuleId} lesson_id={LessonId}",
            DateTime.UtcNow,
            "INFO",
            "lesson.create.succeeded",
            command.CourseId,
            lesson.ModuleId,
            lesson.Id);

        return new Lesson(
            lesson.Id,
            lesson.ModuleId,
            lesson.Title,
            lesson.Content,
            lesson.Position,
            lesson.CreatedAt,
            lesson.UpdatedAt);
    }
}
