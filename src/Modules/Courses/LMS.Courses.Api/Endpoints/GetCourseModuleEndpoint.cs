using LMS.Common.CQRS;
using LMS.Courses.Api.Authorization;
using LMS.Courses.Api.Models;
using LMS.Courses.Application.Models;
using LMS.Courses.Application.Queries.GetCourseModule;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LMS.Courses.Api.Endpoints;

public static class GetCourseModuleEndpoint
{
    public static RouteGroupBuilder MapGetCourseModule(this RouteGroupBuilder group)
    {
        group.MapGet("/{courseId:guid}/modules/{moduleId:guid}", GetCourseModule)
             .WithName(nameof(GetCourseModule))
             .RequireAuthorization(CourseAuthorizationPolicies.CourseMember);

        return group;
    }

    private static async Task<IResult> GetCourseModule(
        Guid courseId,
        Guid moduleId,
        IQueryHandler<GetCourseModuleQuery, CourseModule> handler)
    {
        var result = await handler.Handle(new GetCourseModuleQuery(courseId, moduleId));

        if (result.IsFailure)
        {
            return Results.NotFound();
        }

        var module = result.Value;

        return Results.Ok(new CourseModuleResponse(
            module.Id,
            module.CourseId,
            module.Title,
            module.Description,
            module.Position,
            module.CreatedAt,
            module.UpdatedAt));
    }
}
