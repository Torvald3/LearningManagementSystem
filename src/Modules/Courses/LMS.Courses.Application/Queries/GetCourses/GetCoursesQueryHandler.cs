using LMS.Common.CQRS;
using LMS.Courses.Application.Models;
using LMS.Courses.Core.Services;
using Microsoft.Extensions.Logging;

namespace LMS.Courses.Application.Queries.GetCourses;

public class GetCoursesQueryHandler : IQueryHandler<GetCoursesQuery, IReadOnlyList<Course>>
{
    private readonly ICoursesService _coursesService;
    private readonly ILogger<GetCoursesQueryHandler> _logger;

    public GetCoursesQueryHandler(
        ICoursesService coursesService,
        ILogger<GetCoursesQueryHandler> logger)
    {
        _coursesService = coursesService;
        _logger = logger;
    }

    public async Task<IReadOnlyList<Course>> Handle(
        GetCoursesQuery query,
        CancellationToken cancellationToken = default)
    {
        var courses = await _coursesService.GetCoursesAsync(cancellationToken);

        _logger.LogInformation(
            "timestamp={Timestamp} level={Level} event={Event} courses_count={CoursesCount}",
            DateTime.UtcNow,
            "INFO",
            "courses.get_all.succeeded",
            courses.Count);

        return courses
            .Select(course => new Course(
                course.Id,
                course.AuthorId,
                course.Title,
                course.Theme,
                course.Description,
                course.CreatedAt,
                course.UpdatedAt))
            .ToList();
    }
}