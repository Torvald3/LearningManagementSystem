using LMS.Common.CQRS;
using LMS.Common.Results;
using LMS.Courses.Application.Models;
using LMS.Courses.Core.Services;
using Microsoft.Extensions.Logging;

namespace LMS.Courses.Application.Queries.GetLearningCourses;

public class GetLearningCoursesQueryHandler : IQueryHandler<GetLearningCoursesQuery, List<Course>>
{
    private readonly ICoursesService _coursesService;
    private readonly ILogger<GetLearningCoursesQueryHandler> _logger;

    public GetLearningCoursesQueryHandler(
        ICoursesService coursesService,
        ILogger<GetLearningCoursesQueryHandler> logger)
    {
        _coursesService = coursesService;
        _logger = logger;
    }

    public async Task<Result<List<Course>>> Handle(
        GetLearningCoursesQuery query,
        CancellationToken cancellationToken = default)
    {
        var courses = await _coursesService.GetCoursesByMemberRolesAsync(
            query.UserId,
            [Core.Models.CourseRole.Student],
            cancellationToken);

        _logger.LogInformation(
            "timestamp={Timestamp} level={Level} event={Event} user_id={UserId} courses_count={CoursesCount}",
            DateTime.UtcNow,
            "INFO",
            "courses.get_learning.succeeded",
            query.UserId,
            courses.Count);

        return courses
            .Select(course => new Course(
                course.Id,
                course.Title,
                course.Theme,
                course.Description,
                course.CreatedAt,
                course.UpdatedAt))
            .ToList();
    }
}
