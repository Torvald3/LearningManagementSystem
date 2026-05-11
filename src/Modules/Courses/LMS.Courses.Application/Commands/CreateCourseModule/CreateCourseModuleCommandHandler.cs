using LMS.Common.CQRS;
using LMS.Common.Results;
using LMS.Courses.Application.Errors;
using LMS.Courses.Application.Models;
using LMS.Courses.Core.Services;
using Microsoft.Extensions.Logging;

namespace LMS.Courses.Application.Commands.CreateCourseModule;

public class CreateCourseModuleCommandHandler : ICommandHandler<CreateCourseModuleCommand, CourseModule>
{
    private readonly ICoursesService _coursesService;
    private readonly ILogger<CreateCourseModuleCommandHandler> _logger;

    public CreateCourseModuleCommandHandler(
        ICoursesService coursesService,
        ILogger<CreateCourseModuleCommandHandler> logger)
    {
        _coursesService = coursesService;
        _logger = logger;
    }

    public async Task<Result<CourseModule>> HandleAsync(
        CreateCourseModuleCommand command,
        CancellationToken cancellationToken = default)
    {
        var course = await _coursesService.GetCourseAsync(command.CourseId, cancellationToken);

        if (course is null)
        {
            _logger.LogWarning(
                "timestamp={Timestamp} level={Level} event={Event} course_id={CourseId}",
                DateTime.UtcNow,
                "WARN",
                "course_module.create.course_not_found",
                command.CourseId);

            return CourseErrors.CourseNotFound(command.CourseId);
        }

        var now = DateTime.UtcNow;
        var module = new LMS.Courses.Core.Models.CourseModule
        {
            Id = Guid.NewGuid(),
            CourseId = command.CourseId,
            Title = command.Title.Trim(),
            Description = command.Description.Trim(),
            Position = await _coursesService.GetNextCourseModulePositionAsync(command.CourseId, cancellationToken),
            CreatedAt = now,
            UpdatedAt = now
        };

        await _coursesService.CreateCourseModuleAsync(module, cancellationToken);

        _logger.LogInformation(
            "timestamp={Timestamp} level={Level} event={Event} course_id={CourseId} module_id={ModuleId}",
            DateTime.UtcNow,
            "INFO",
            "course_module.create.succeeded",
            module.CourseId,
            module.Id);

        return new CourseModule(
            module.Id,
            module.CourseId,
            module.Title,
            module.Description,
            module.Position,
            module.CreatedAt,
            module.UpdatedAt);
    }
}
