using FluentValidation;
using LMS.Common.Authorization;
using LMS.Common.CQRS;
using LMS.Courses.Api.Authorization;
using LMS.Courses.Api.Models;
using LMS.Courses.Application.Commands.AddCourseMember;
using LMS.Courses.Core.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using CourseMemberModel = LMS.Courses.Application.Models.CourseMember;

namespace LMS.Courses.Api.Endpoints;

public static class AddCourseMemberEndpoint
{
    public static RouteGroupBuilder MapAddCourseMember(this RouteGroupBuilder group)
    {
        group.MapPost("/{courseId:guid}/members", AddCourseMember)
             .WithName(nameof(AddCourseMember))
             .RequireAuthorization(CourseAuthorizationPolicies.CourseEditor);

        return group;
    }

    private static async Task<IResult> AddCourseMember(
        Guid courseId,
        AddCourseMemberRequest request,
        IValidator<AddCourseMemberRequest> validator,
        ICurrentUserService currentUserService,
        ICourseAuthorizationService courseAuthorizationService,
        ICommandHandler<AddCourseMemberCommand, CourseMemberModel> handler)
    {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return Results.BadRequest(validationResult.Errors);
        }

        if (!Enum.TryParse<CourseRole>(request.Role, ignoreCase: true, out var role))
        {
            IEnumerable<string> errors = ["Role must be CourseOwner, Teacher, or Student."];
            return Results.BadRequest(errors);
        }

        if (currentUserService.UserId is not { } currentUserId)
        {
            return Results.Unauthorized();
        }

        var canAddMember = role switch
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

        if (!canAddMember)
        {
            return Results.Forbid();
        }

        var result = await handler.HandleAsync(
            new AddCourseMemberCommand(
                courseId,
                request.UserId,
                role));

        if (result.IsFailure)
        {
            return CourseEndpointResults.FromError(result.Error);
        }

        var member = result.Value;

        return Results.Created(
            $"/api/courses/{courseId}/members/{member.UserId}",
            new CourseMemberResponse(
                member.Id,
                member.CourseId,
                member.UserId,
                member.Role.ToString(),
                member.CreatedAt,
                member.UpdatedAt));
    }
}
