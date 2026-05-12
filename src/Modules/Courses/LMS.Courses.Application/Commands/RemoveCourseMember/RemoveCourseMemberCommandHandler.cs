using LMS.Common.CQRS;
using LMS.Common.Results;
using LMS.Courses.Application.Errors;
using LMS.Courses.Core.Models;
using LMS.Courses.Core.Services;
using Microsoft.Extensions.Logging;

namespace LMS.Courses.Application.Commands.RemoveCourseMember;

public class RemoveCourseMemberCommandHandler : ICommandHandler<RemoveCourseMemberCommand>
{
    private readonly ICoursesService _coursesService;
    private readonly ILogger<RemoveCourseMemberCommandHandler> _logger;

    public RemoveCourseMemberCommandHandler(
        ICoursesService coursesService,
        ILogger<RemoveCourseMemberCommandHandler> logger)
    {
        _coursesService = coursesService;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(
        RemoveCourseMemberCommand command,
        CancellationToken cancellationToken = default)
    {
        var course = await _coursesService.GetCourseAsync(command.CourseId, cancellationToken);

        if (course is null)
        {
            return CourseErrors.CourseNotFound(command.CourseId);
        }

        var member = await _coursesService.GetCourseMemberAsync(
            command.CourseId,
            command.UserId,
            cancellationToken);

        if (member is null)
        {
            return CourseErrors.CourseMemberNotFound(command.CourseId, command.UserId);
        }

        if (member.Role == CourseRole.CourseOwner)
        {
            return CourseErrors.CannotRemoveCourseOwner(command.CourseId);
        }

        var removed = await _coursesService.DeleteCourseMemberAsync(
            command.CourseId,
            command.UserId,
            cancellationToken);

        if (!removed)
        {
            return CourseErrors.CourseMemberNotFound(command.CourseId, command.UserId);
        }

        _logger.LogInformation(
            "timestamp={Timestamp} level={Level} event={Event} course_id={CourseId} user_id={UserId} role={Role}",
            DateTime.UtcNow,
            "INFO",
            "course_member.remove.succeeded",
            command.CourseId,
            command.UserId,
            member.Role);

        return Result.Success;
    }
}
