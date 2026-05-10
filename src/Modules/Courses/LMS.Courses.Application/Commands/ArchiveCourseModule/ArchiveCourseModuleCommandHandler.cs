using LMS.Common.CQRS;
using LMS.Courses.Core.Services;
using Microsoft.Extensions.Logging;

namespace LMS.Courses.Application.Commands.ArchiveCourseModule;

public class ArchiveCourseModuleCommandHandler : ICommandHandler<ArchiveCourseModuleCommand, ArchiveCourseModuleResult>
{
    private readonly ICoursesService _coursesService;
    private readonly ILogger<ArchiveCourseModuleCommandHandler> _logger;

    public ArchiveCourseModuleCommandHandler(
        ICoursesService coursesService,
        ILogger<ArchiveCourseModuleCommandHandler> logger)
    {
        _coursesService = coursesService;
        _logger = logger;
    }

    public async Task<ArchiveCourseModuleResult> HandleAsync(
        ArchiveCourseModuleCommand command,
        CancellationToken cancellationToken = default)
    {
        var course = await _coursesService.GetCourseAsync(command.CourseId, cancellationToken);

        if (course is null)
        {
            _logger.LogWarning(
                "timestamp={Timestamp} level={Level} event={Event} course_id={CourseId}",
                DateTime.UtcNow,
                "WARN",
                "course_module.archive.course_not_found",
                command.CourseId);

            return new ArchiveCourseModuleResult(
                ArchiveCourseModuleStatus.CourseNotFound,
                ["Course not found."]);
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
                "course_module.archive.module_not_found",
                command.CourseId,
                command.ModuleId);

            return new ArchiveCourseModuleResult(
                ArchiveCourseModuleStatus.ModuleNotFound,
                ["Module not found."]);
        }

        var archived = await _coursesService.ArchiveCourseModuleAsync(
            command.CourseId,
            command.ModuleId,
            DateTime.UtcNow,
            cancellationToken);

        if (!archived)
        {
            return new ArchiveCourseModuleResult(
                ArchiveCourseModuleStatus.ModuleNotFound,
                ["Module not found."]);
        }

        _logger.LogInformation(
            "timestamp={Timestamp} level={Level} event={Event} course_id={CourseId} module_id={ModuleId}",
            DateTime.UtcNow,
            "INFO",
            "course_module.archive.succeeded",
            command.CourseId,
            command.ModuleId);

        return new ArchiveCourseModuleResult(
            ArchiveCourseModuleStatus.Success,
            []);
    }
}
