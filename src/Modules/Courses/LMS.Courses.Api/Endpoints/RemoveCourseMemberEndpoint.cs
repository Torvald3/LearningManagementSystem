using LMS.Common.Authorization;
using LMS.Common.CQRS;
using LMS.Courses.Api.Authorization;
using LMS.Courses.Application.Commands.RemoveCourseMember;
using LMS.Courses.Core.Models;
using LMS.Courses.Core.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LMS.Courses.Api.Endpoints;

public static class RemoveCourseMemberEndpoint
{
    public static RouteGroupBuilder MapRemoveCourseMember(this RouteGroupBuilder group)
    {
        group.MapDelete("/{courseId:guid}/members/{userId:guid}", RemoveCourseMember)
             .WithName(nameof(RemoveCourseMember))
             .RequireAuthorization(CourseAuthorizationPolicies.CourseEditor);

        return group;
    }

    private static async Task<IResult> RemoveCourseMember(
        Guid courseId,
        Guid userId,
        ICurrentUserService currentUserService,
        ICoursesService coursesService,
        ICourseAuthorizationService courseAuthorizationService,
        ICommandHandler<RemoveCourseMemberCommand> handler)
    {
        if (currentUserService.UserId is not { } currentUserId)
        {
            return Results.Unauthorized();
        }

        var targetMember = await coursesService.GetCourseMemberAsync(courseId, userId);

        if (targetMember is null)
        {
            return Results.NotFound();
        }

        var canRemoveMember = targetMember.Role switch
        {
            CourseRole.Teacher => await courseAuthorizationService.HasAnyRoleAsync(
                courseId,
                currentUserId,
                CourseRole.CourseOwner),
            CourseRole.Student => await courseAuthorizationService.HasAnyRoleAsync(
                courseId,
                currentUserId,
                CourseRole.CourseOwner,
                CourseRole.Teacher),
            _ => false
        };

        if (!canRemoveMember)
        {
            return Results.Forbid();
        }

        var result = await handler.HandleAsync(new RemoveCourseMemberCommand(courseId, userId));

        return result.IsSuccess
            ? Results.NoContent()
            : CourseEndpointResults.FromError(result.Error);
    }
}
