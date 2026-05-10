using LMS.Common.CQRS;
using LMS.Courses.Application.Models;
using LMS.Courses.Core.Services;
using Microsoft.Extensions.Logging;

namespace LMS.Courses.Application.Queries.GetLesson;

public class GetLessonQueryHandler : IQueryHandler<GetLessonQuery, Lesson?>
{
    private readonly ICoursesService _coursesService;
    private readonly ILogger<GetLessonQueryHandler> _logger;

    public GetLessonQueryHandler(
        ICoursesService coursesService,
        ILogger<GetLessonQueryHandler> logger)
    {
        _coursesService = coursesService;
        _logger = logger;
    }

    public async Task<Lesson?> Handle(GetLessonQuery query, CancellationToken cancellationToken = default)
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
                "lesson.get.module_not_found",
                query.CourseId,
                query.ModuleId);

            return null;
        }

        var lesson = await _coursesService.GetLessonAsync(
            query.ModuleId,
            query.LessonId,
            cancellationToken);

        if (lesson is null)
        {
            _logger.LogWarning(
                "timestamp={Timestamp} level={Level} event={Event} module_id={ModuleId} lesson_id={LessonId}",
                DateTime.UtcNow,
                "WARN",
                "lesson.get.not_found",
                query.ModuleId,
                query.LessonId);

            return null;
        }

        _logger.LogInformation(
            "timestamp={Timestamp} level={Level} event={Event} module_id={ModuleId} lesson_id={LessonId}",
            DateTime.UtcNow,
            "INFO",
            "lesson.get.succeeded",
            query.ModuleId,
            query.LessonId);

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
