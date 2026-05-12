using LMS.Common.CQRS;
using LMS.Courses.Api.Authorization;
using LMS.Courses.Api.Models;
using LMS.Courses.Application.Queries.GetCourseMembers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using CourseMemberModel = LMS.Courses.Application.Models.CourseMember;

namespace LMS.Courses.Api.Endpoints;

public static class GetCourseMembersEndpoint
{
    public static RouteGroupBuilder MapGetCourseMembers(this RouteGroupBuilder group)
    {
        group.MapGet("/{courseId:guid}/members", GetCourseMembers)
             .WithName(nameof(GetCourseMembers))
             .RequireAuthorization(CourseAuthorizationPolicies.CourseMember);

        return group;
    }

    private static async Task<IResult> GetCourseMembers(
        Guid courseId,
        IQueryHandler<GetCourseMembersQuery, List<CourseMemberModel>> handler)
    {
        var result = await handler.Handle(new GetCourseMembersQuery(courseId));

        if (result.IsFailure)
        {
            return Results.NotFound();
        }

        var response = result.Value
            .Select(member => new CourseMemberResponse(
                member.Id,
                member.CourseId,
                member.UserId,
                member.Role.ToString(),
                member.CreatedAt,
                member.UpdatedAt))
            .ToList();

        return Results.Ok(response);
    }
}
