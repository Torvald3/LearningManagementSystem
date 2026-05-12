using LMS.Common.CQRS;
using LMS.Common.Results;
using LMS.Courses.Application.Errors;
using LMS.Courses.Core.Models;
using LMS.Courses.Core.Services;
using LMS.Users.Contracts.Services;
using Microsoft.Extensions.Logging;
using CourseMemberModel = LMS.Courses.Application.Models.CourseMember;

namespace LMS.Courses.Application.Commands.AddCourseMember;

public class AddCourseMemberCommandHandler : ICommandHandler<AddCourseMemberCommand, CourseMemberModel>
{
    private readonly ICoursesService _coursesService;
    private readonly IUsersModuleService _usersModuleService;
    private readonly ILogger<AddCourseMemberCommandHandler> _logger;

    public AddCourseMemberCommandHandler(
        ICoursesService coursesService,
        IUsersModuleService usersModuleService,
        ILogger<AddCourseMemberCommandHandler> logger)
    {
        _coursesService = coursesService;
        _usersModuleService = usersModuleService;
        _logger = logger;
    }

    public async Task<Result<CourseMemberModel>> HandleAsync(
        AddCourseMemberCommand command,
        CancellationToken cancellationToken = default)
    {
        var course = await _coursesService.GetCourseAsync(command.CourseId, cancellationToken);

        if (course is null)
        {
            return CourseErrors.CourseNotFound(command.CourseId);
        }

        var userExists = await _usersModuleService.UserExistsAsync(command.UserId);

        if (!userExists)
        {
            return CourseErrors.UserNotFound(command.UserId);
        }

        var existingMember = await _coursesService.GetCourseMemberAsync(
            command.CourseId,
            command.UserId,
            cancellationToken);

        if (existingMember is not null)
        {
            return CourseErrors.CourseMemberAlreadyExists(command.CourseId, command.UserId);
        }

        if (command.Role == CourseRole.CourseOwner)
        {
            var ownerExists = await _coursesService.CourseOwnerExistsAsync(command.CourseId, cancellationToken);

            if (ownerExists)
            {
                return CourseErrors.CourseOwnerAlreadyExists(command.CourseId);
            }
        }

        var now = DateTime.UtcNow;
        var member = new LMS.Courses.Core.Models.CourseMember
        {
            Id = Guid.NewGuid(),
            CourseId = command.CourseId,
            UserId = command.UserId,
            Role = command.Role,
            CreatedAt = now,
            UpdatedAt = now
        };

        await _coursesService.CreateCourseMemberAsync(member, cancellationToken);

        _logger.LogInformation(
            "timestamp={Timestamp} level={Level} event={Event} course_id={CourseId} user_id={UserId} role={Role}",
            DateTime.UtcNow,
            "INFO",
            "course_member.add.succeeded",
            member.CourseId,
            member.UserId,
            member.Role);

        return new CourseMemberModel(
            member.Id,
            member.CourseId,
            member.UserId,
            member.Role,
            member.CreatedAt,
            member.UpdatedAt);
    }
}
