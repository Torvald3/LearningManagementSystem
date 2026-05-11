using LMS.Common.CQRS;
using LMS.Common.Observability.Metrics;
using LMS.Common.Results;
using LMS.Courses.Application.Errors;
using LMS.Courses.Core.Services;
using Microsoft.Extensions.Logging;

namespace LMS.Courses.Application.Commands.DeleteCourse;

public class DeleteCourseCommandHandler : ICommandHandler<DeleteCourseCommand>
{
    private readonly ICoursesService _coursesService;
    private readonly ILogger<DeleteCourseCommandHandler> _logger;
    private readonly AppMetrics _appMetrics;

    public DeleteCourseCommandHandler(
        ICoursesService coursesService,
        ILogger<DeleteCourseCommandHandler> logger,
        AppMetrics appMetrics)
    {
        _coursesService = coursesService;
        _logger = logger;
        _appMetrics = appMetrics;
    }

    public async Task<Result> HandleAsync(DeleteCourseCommand command, CancellationToken cancellationToken = default)
    {
        var deleted = await _coursesService.DeleteCourseAsync(command.CourseId, cancellationToken);

        if (!deleted)
        {
            _logger.LogWarning(
                "timestamp={Timestamp} level={Level} event={Event} course_id={CourseId}",
                DateTime.UtcNow,
                "WARN",
                "course.delete.not_found",
                command.CourseId);

            return CourseErrors.CourseNotFound(command.CourseId);
        }

        _logger.LogInformation(
            "timestamp={Timestamp} level={Level} event={Event} course_id={CourseId}",
            DateTime.UtcNow,
            "INFO",
            "course.delete.succeeded",
            command.CourseId);

        _appMetrics.CourseDeleted(command.CourseId);

        return Result.Success;
    }
}
