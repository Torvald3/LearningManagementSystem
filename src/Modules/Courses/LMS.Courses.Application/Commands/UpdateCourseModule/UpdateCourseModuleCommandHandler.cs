using LMS.Common.CQRS;
using LMS.Common.Results;
using LMS.Courses.Application.Errors;
using LMS.Courses.Application.Models;
using LMS.Courses.Core.Services;
using Microsoft.Extensions.Logging;

namespace LMS.Courses.Application.Commands.UpdateCourseModule;

public class UpdateCourseModuleCommandHandler : ICommandHandler<UpdateCourseModuleCommand, CourseModule>
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

    public async Task<Result<CourseModule>> HandleAsync(
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

            return CourseErrors.CourseNotFound(command.CourseId);
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

            return CourseErrors.ModuleNotFound(command.ModuleId);
        }

        var modulesCount = await _coursesService.GetCourseModulesCountAsync(command.CourseId, cancellationToken);

        if (command.Position < 1 || command.Position > modulesCount)
        {
            return CourseErrors.InvalidPosition(modulesCount);
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
            return CourseErrors.ModuleNotFound(command.ModuleId);
        }

        _logger.LogInformation(
            "timestamp={Timestamp} level={Level} event={Event} course_id={CourseId} module_id={ModuleId} position={Position}",
            DateTime.UtcNow,
            "INFO",
            "course_module.update.succeeded",
            updatedModule.CourseId,
            updatedModule.Id,
            updatedModule.Position);

        return new CourseModule(
            updatedModule.Id,
            updatedModule.CourseId,
            updatedModule.Title,
            updatedModule.Description,
            updatedModule.Position,
            updatedModule.CreatedAt,
            updatedModule.UpdatedAt);
    }
}
