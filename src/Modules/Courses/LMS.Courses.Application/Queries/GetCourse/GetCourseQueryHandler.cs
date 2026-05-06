using LMS.Common.CQRS;
using LMS.Courses.Application.Models;
using LMS.Courses.Core.Services;
using Microsoft.Extensions.Logging;

namespace LMS.Courses.Application.Queries.GetCourse;

public class GetCourseQueryHandler : IQueryHandler<GetCourseQuery, Course?>
{
    private readonly ICoursesService _coursesService;
    private readonly ILogger<GetCourseQueryHandler> _logger;

    public GetCourseQueryHandler(
        ICoursesService coursesService,
        ILogger<GetCourseQueryHandler> logger)
    {
        _coursesService = coursesService;
        _logger = logger;
    }

    public async Task<Course?> Handle(GetCourseQuery query, CancellationToken cancellationToken = default)
    {
        var course = await _coursesService.GetCourseAsync(query.CourseId, cancellationToken);

        if (course is null)
        {
            _logger.LogWarning(
                "timestamp={Timestamp} level={Level} event={Event} course_id={CourseId}",
                DateTime.UtcNow,
                "WARN",
                "course.get.not_found",
                query.CourseId);

            return null;
        }

        _logger.LogInformation(
            "timestamp={Timestamp} level={Level} event={Event} course_id={CourseId}",
            DateTime.UtcNow,
            "INFO",
            "course.get.succeeded",
            query.CourseId);

        return new Course(
            course.Id,
            course.AuthorId,
            course.Title,
            course.Theme,
            course.Description,
            course.CreatedAt,
            course.UpdatedAt);
    }
}
