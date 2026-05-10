using LMS.Common.CQRS;
using LMS.Courses.Application.Models;
using LMS.Courses.Core.Services;
using Microsoft.Extensions.Logging;

namespace LMS.Courses.Application.Queries.GetCourseModules;

public class GetCourseModulesQueryHandler : IQueryHandler<GetCourseModulesQuery, IReadOnlyList<CourseModuleSummary>?>
{
    private readonly ICoursesService _coursesService;
    private readonly ILogger<GetCourseModulesQueryHandler> _logger;

    public GetCourseModulesQueryHandler(
        ICoursesService coursesService,
        ILogger<GetCourseModulesQueryHandler> logger)
    {
        _coursesService = coursesService;
        _logger = logger;
    }

    public async Task<IReadOnlyList<CourseModuleSummary>?> Handle(
        GetCourseModulesQuery query,
        CancellationToken cancellationToken = default)
    {
        var course = await _coursesService.GetCourseAsync(query.CourseId, cancellationToken);

        if (course is null)
        {
            _logger.LogWarning(
                "timestamp={Timestamp} level={Level} event={Event} course_id={CourseId}",
                DateTime.UtcNow,
                "WARN",
                "course_modules.get.course_not_found",
                query.CourseId);

            return null;
        }

        var modules = await _coursesService.GetCourseModulesAsync(query.CourseId, cancellationToken);

        _logger.LogInformation(
            "timestamp={Timestamp} level={Level} event={Event} course_id={CourseId} modules_count={ModulesCount}",
            DateTime.UtcNow,
            "INFO",
            "course_modules.get.succeeded",
            query.CourseId,
            modules.Count);

        return modules
            .Select(module => new CourseModuleSummary(
                module.Id,
                module.CourseId,
                module.Title,
                module.Description,
                module.Position,
                module.LessonsCount))
            .ToList();
    }
}
