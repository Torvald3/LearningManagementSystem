using LMS.Common.CQRS;
using LMS.Courses.Application.Models;
using LMS.Courses.Core.Services;
using Microsoft.Extensions.Logging;

namespace LMS.Courses.Application.Queries.GetLessons;

public class GetLessonsQueryHandler : IQueryHandler<GetLessonsQuery, IReadOnlyList<LessonSummary>?>
{
    private readonly ICoursesService _coursesService;
    private readonly ILogger<GetLessonsQueryHandler> _logger;

    public GetLessonsQueryHandler(
        ICoursesService coursesService,
        ILogger<GetLessonsQueryHandler> logger)
    {
        _coursesService = coursesService;
        _logger = logger;
    }

    public async Task<IReadOnlyList<LessonSummary>?> Handle(
        GetLessonsQuery query,
        CancellationToken cancellationToken = default)
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
                "lessons.get.module_not_found",
                query.CourseId,
                query.ModuleId);

            return null;
        }

        var lessons = await _coursesService.GetLessonsAsync(query.ModuleId, cancellationToken);

        _logger.LogInformation(
            "timestamp={Timestamp} level={Level} event={Event} module_id={ModuleId} lessons_count={LessonsCount}",
            DateTime.UtcNow,
            "INFO",
            "lessons.get.succeeded",
            query.ModuleId,
            lessons.Count);

        return lessons
            .Select(lesson => new LessonSummary(
                lesson.Id,
                lesson.ModuleId,
                lesson.Title,
                lesson.Position))
            .ToList();
    }
}
