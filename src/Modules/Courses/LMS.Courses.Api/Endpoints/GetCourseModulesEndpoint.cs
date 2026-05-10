using LMS.Common.CQRS;
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
             .WithName(nameof(GetCourseModules));

        return group;
    }

    private static async Task<IResult> GetCourseModules(
        Guid courseId,
        IQueryHandler<GetCourseModulesQuery, IReadOnlyList<CourseModuleSummary>?> handler)
    {
        var modules = await handler.Handle(new GetCourseModulesQuery(courseId));

        if (modules is null)
        {
            return Results.NotFound();
        }

        var response = modules
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
