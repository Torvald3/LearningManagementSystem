using LMS.Common.CQRS;
using LMS.Common.Observability.Metrics;
using LMS.Courses.Core.Services;
using Microsoft.Extensions.Logging;

namespace LMS.Courses.Application.Commands.DeleteCourse;

public class DeleteCourseCommandHandler : ICommandHandler<DeleteCourseCommand, bool>
{
    private readonly ICoursesService _coursesService;
    private readonly ILogger<DeleteCourseCommandHandler> _logger;
    private readonly AppMetrics _appMetrics;

    public DeleteCourseCommandHandler(ICoursesService coursesService, 
                                      ILogger<DeleteCourseCommandHandler> logger, 
                                      AppMetrics appMetrics)
    {
        _coursesService = coursesService;
        _logger = logger;
        _appMetrics = appMetrics;
    }

    public async Task<bool> HandleAsync(DeleteCourseCommand command, CancellationToken cancellationToken = default)
    {
        var result = await _coursesService.DeleteCourseAsync(command.CourseId, cancellationToken);

        if (!result)
        {
            _logger.LogWarning(
                "timestamp={Timestamp} level={Level} event={Event} course_id={CourseId}",
                DateTime.UtcNow,
                "WARN",
                "course.delete.not_found",
                command.CourseId);
        }
        else
        {
            _logger.LogInformation(
                "timestamp={Timestamp} level={Level} event={Event} course_id={CourseId}",
                DateTime.UtcNow,
                "INFO",
                "course.delete.succeeded",
                command.CourseId);
            
            _appMetrics.CourseDeleted(command.CourseId);
        }
        
        return result;
    }
}