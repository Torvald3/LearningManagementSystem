using LMS.Common.CQRS;
using LMS.Courses.Application.Models;
using LMS.Courses.Core.Services;
using Microsoft.Extensions.Logging;

namespace LMS.Courses.Application.Commands.UpdateCourse;

public class UpdateCourseCommandHandler : ICommandHandler<UpdateCourseCommand, UpdateCourseResult>
{
    private readonly ICoursesService _coursesService;
    private readonly ILogger<UpdateCourseCommandHandler> _logger;

    public UpdateCourseCommandHandler(
        ICoursesService coursesService,
        ILogger<UpdateCourseCommandHandler> logger)
    {
        _coursesService = coursesService;
        _logger = logger;
    }

    public async Task<UpdateCourseResult> HandleAsync(
        UpdateCourseCommand command,
        CancellationToken cancellationToken = default)
    {
        var existingCourse = await _coursesService.GetCourseAsync(command.CourseId, cancellationToken);

        if (existingCourse is null)
        {
            _logger.LogWarning(
                "timestamp={Timestamp} level={Level} event={Event} course_id={CourseId}",
                DateTime.UtcNow,
                "WARN",
                "course.update.not_found",
                command.CourseId);

            return new UpdateCourseResult(
                UpdateCourseStatus.NotFound,
                null,
                ["Course not found."]);
        }

        var updatedCourse = new Core.Models.Course
        {
            Id = existingCourse.Id,
            AuthorId = existingCourse.AuthorId,
            CreatedAt = existingCourse.CreatedAt,
            Title = command.Title.Trim(),
            Theme = command.Theme.Trim(),
            Description = command.Description.Trim(),
            UpdatedAt = DateTime.UtcNow
        };

        await _coursesService.UpdateCourseAsync(updatedCourse, cancellationToken);

        _logger.LogInformation(
            "timestamp={Timestamp} level={Level} event={Event} course_id={CourseId}",
            DateTime.UtcNow,
            "INFO",
            "course.update.succeeded",
            command.CourseId);

        return new UpdateCourseResult(
            UpdateCourseStatus.Success,
            new Course(
                updatedCourse.Id,
                updatedCourse.AuthorId,
                updatedCourse.Title,
                updatedCourse.Theme,
                updatedCourse.Description,
                updatedCourse.CreatedAt,
                updatedCourse.UpdatedAt),
            []);
    }
}