using LMS.Common.CQRS;
using LMS.Common.Results;
using LMS.Courses.Application.Models;
using LMS.Courses.Core.Services;
using Microsoft.Extensions.Logging;

namespace LMS.Courses.Application.Queries.GetCourses;

public class GetCoursesQueryHandler : IQueryHandler<GetCoursesQuery, List<Course>>
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

    public async Task<Result<List<Course>>> Handle(
        GetCoursesQuery query,
        CancellationToken cancellationToken = default)
    {
        var courses = await _coursesService.GetCoursesByMemberAsync(query.UserId, cancellationToken);

        _logger.LogInformation(
            "timestamp={Timestamp} level={Level} event={Event} user_id={UserId} courses_count={CoursesCount}",
            DateTime.UtcNow,
            "INFO",
            "courses.get_by_member.succeeded",
            query.UserId,
            courses.Count);

        List<Course> result = courses
            .Select(course => new Course(
                course.Id,
                course.AuthorId,
                course.Title,
                course.Theme,
                course.Description,
                course.CreatedAt,
                course.UpdatedAt))
            .ToList();

        return result;
    }
}
