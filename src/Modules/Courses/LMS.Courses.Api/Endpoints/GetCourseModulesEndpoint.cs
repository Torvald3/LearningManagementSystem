using LMS.Common.CQRS;
using LMS.Courses.Api.Authorization;
using LMS.Courses.Api.Models;
using LMS.Courses.Application.Models;
using LMS.Courses.Application.Queries.GetCourseModules;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LMS.Courses.Api.Endpoints;

public static class GetCourseModulesEndpoint
{
    public static RouteGroupBuilder MapGetCourseModules(this RouteGroupBuilder group)
    {
        group.MapGet("/{courseId:guid}/modules", GetCourseModules)
             .WithName(nameof(GetCourseModules))
             .RequireAuthorization(CourseAuthorizationPolicies.CourseMember);

        return group;
    }

    private static async Task<IResult> GetCourseModules(
        Guid courseId,
        IQueryHandler<GetCourseModulesQuery, List<CourseModuleSummary>> handler)
    {
        var result = await handler.Handle(new GetCourseModulesQuery(courseId));

        if (result.IsFailure)
        {
            return Results.NotFound();
        }

        var response = result.Value
            .Select(module => new CourseModuleSummaryResponse(
                module.Id,
                module.CourseId,
                module.Title,
                module.Description,
                module.Position,
                module.LessonsCount))
            .ToList();

        return Results.Ok(response);
    }
}
