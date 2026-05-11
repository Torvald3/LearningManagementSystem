using LMS.Common.CQRS;
using LMS.Common.Results;
using LMS.Courses.Application.Errors;
using LMS.Courses.Application.Models;
using LMS.Courses.Core.Services;
using Microsoft.Extensions.Logging;

namespace LMS.Courses.Application.Queries.GetCourseModule;

public class GetCourseModuleQueryHandler : IQueryHandler<GetCourseModuleQuery, CourseModule>
{
    private readonly ICoursesService _coursesService;
    private readonly ILogger<GetCourseModuleQueryHandler> _logger;

    public GetCourseModuleQueryHandler(
        ICoursesService coursesService,
        ILogger<GetCourseModuleQueryHandler> logger)
    {
        _coursesService = coursesService;
        _logger = logger;
    }

    public async Task<Result<CourseModule>> Handle(GetCourseModuleQuery query, CancellationToken cancellationToken = default)
    {
        var module = await _coursesService.GetCourseModuleAsync(
            query.CourseId,
            query.ModuleId,
            cancellationToken);

        if (module is null)
        {
            _logger.LogWarning(
                "timestamp={Timestamp} level={Level} event={Event} course_id={CourseId} module_id={ModuleId}",
                DateTime.UtcNow,
                "WARN",
                "course_module.get.not_found",
                query.CourseId,
                query.ModuleId);

            return CourseErrors.ModuleNotFound(query.ModuleId);
        }

        _logger.LogInformation(
            "timestamp={Timestamp} level={Level} event={Event} course_id={CourseId} module_id={ModuleId}",
            DateTime.UtcNow,
            "INFO",
            "course_module.get.succeeded",
            query.CourseId,
            query.ModuleId);

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
