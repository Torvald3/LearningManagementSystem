using LMS.Common.CQRS;
using LMS.Common.Observability.Metrics;
using LMS.Courses.Application.Models;
using LMS.Courses.Core.Services;
using LMS.Users.Contracts.Services;
using Microsoft.Extensions.Logging;

namespace LMS.Courses.Application.Commands.CreateCourse;

public class CreateCourseCommandHandler : ICommandHandler<CreateCourseCommand, CreateCourseResult>
{
    private readonly ICoursesService _coursesService;
    private readonly IUsersModuleService _usersModuleService;
    private readonly ILogger<CreateCourseCommandHandler> _logger;
    private readonly AppMetrics _metrics;

    public CreateCourseCommandHandler(
        ICoursesService coursesService,
        IUsersModuleService usersModuleService,
        ILogger<CreateCourseCommandHandler> logger,
        AppMetrics metrics)
    {
        _coursesService = coursesService;
        _usersModuleService = usersModuleService;
        _logger = logger;
        _metrics = metrics;
    }

    public async Task<CreateCourseResult> HandleAsync(CreateCourseCommand command, CancellationToken cancellationToken = default)
    {
        var authorExists = await _usersModuleService.UserExistsAsync(command.AuthorId);

        if (!authorExists)
        {
            return new CreateCourseResult(
                CreateCourseStatus.AuthorNotFound,
                null,
                ["Author does not exist."]);
        }

        var now = DateTime.UtcNow;

        var course = new Core.Models.Course
        {
            Id = Guid.NewGuid(),
            AuthorId = command.AuthorId,
            Title = command.Title,
            Theme = command.Theme,
            Description = command.Description,
            CreatedAt = now,
            UpdatedAt = now
        };

        await _coursesService.CreateCourseAsync(course, cancellationToken);

        _metrics.CourseCreated(course.Id);

        _logger.LogInformation(
            "timestamp={Timestamp} level={Level} event={Event} course_id={CourseId} author_id={AuthorId}",
            DateTime.UtcNow,
            "INFO",
            "course.create.succeeded",
            course.Id,
            course.AuthorId);

        return new CreateCourseResult(
            CreateCourseStatus.Success,
            new Course(course.Id, course.AuthorId, course.Title, course.Theme, course.Description, course.CreatedAt, course.UpdatedAt),
            []);
    }
}
