using LMS.Common.CQRS;
using LMS.Courses.Application.Models;
using LMS.Courses.Core.Services;
using Microsoft.Extensions.Logging;

namespace LMS.Courses.Application.Commands.UpdateCourseModule;

public class UpdateCourseModuleCommandHandler : ICommandHandler<UpdateCourseModuleCommand, UpdateCourseModuleResult>
{
    private readonly ICoursesService _coursesService;
    private readonly ILogger<UpdateCourseModuleCommandHandler> _logger;

    public UpdateCourseModuleCommandHandler(
        ICoursesService coursesService,
        ILogger<UpdateCourseModuleCommandHandler> logger)
    {
        _coursesService = coursesService;
        _logger = logger;
    }

    public async Task<UpdateCourseModuleResult> HandleAsync(
        UpdateCourseModuleCommand command,
        CancellationToken cancellationToken = default)
    {
        var course = await _coursesService.GetCourseAsync(command.CourseId, cancellationToken);

        if (course is null)
        {
            _logger.LogWarning(
                "timestamp={Timestamp} level={Level} event={Event} course_id={CourseId}",
                DateTime.UtcNow,
                "WARN",
                "course_module.update.course_not_found",
                command.CourseId);

            return new UpdateCourseModuleResult(
                UpdateCourseModuleStatus.CourseNotFound,
                null,
                ["Course not found."]);
        }

        var existingModule = await _coursesService.GetCourseModuleAsync(
            command.CourseId,
            command.ModuleId,
            cancellationToken);

        if (existingModule is null)
        {
            _logger.LogWarning(
                "timestamp={Timestamp} level={Level} event={Event} course_id={CourseId} module_id={ModuleId}",
                DateTime.UtcNow,
                "WARN",
                "course_module.update.module_not_found",
                command.CourseId,
                command.ModuleId);

            return new UpdateCourseModuleResult(
                UpdateCourseModuleStatus.ModuleNotFound,
                null,
                ["Module not found."]);
        }

        var modulesCount = await _coursesService.GetCourseModulesCountAsync(command.CourseId, cancellationToken);

        if (command.Position < 1 || command.Position > modulesCount)
        {
            return new UpdateCourseModuleResult(
                UpdateCourseModuleStatus.InvalidPosition,
                null,
                [$"Position must be between 1 and {modulesCount}."]);
        }

        var updatedModule = new LMS.Courses.Core.Models.CourseModule
        {
            Id = existingModule.Id,
            CourseId = existingModule.CourseId,
            Title = command.Title.Trim(),
            Description = command.Description.Trim(),
            Position = command.Position,
            CreatedAt = existingModule.CreatedAt,
            UpdatedAt = DateTime.UtcNow
        };

        var updated = await _coursesService.UpdateCourseModuleAsync(updatedModule, cancellationToken);

        if (!updated)
        {
            return new UpdateCourseModuleResult(
                UpdateCourseModuleStatus.ModuleNotFound,
                null,
                ["Module not found."]);
        }

        _logger.LogInformation(
            "timestamp={Timestamp} level={Level} event={Event} course_id={CourseId} module_id={ModuleId} position={Position}",
            DateTime.UtcNow,
            "INFO",
            "course_module.update.succeeded",
            updatedModule.CourseId,
            updatedModule.Id,
            updatedModule.Position);

        return new UpdateCourseModuleResult(
            UpdateCourseModuleStatus.Success,
            new CourseModule(
                updatedModule.Id,
                updatedModule.CourseId,
                updatedModule.Title,
                updatedModule.Description,
                updatedModule.Position,
                updatedModule.CreatedAt,
                updatedModule.UpdatedAt),
            []);
    }
}
