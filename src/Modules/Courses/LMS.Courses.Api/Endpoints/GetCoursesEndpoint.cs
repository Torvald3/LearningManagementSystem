using LMS.Common.CQRS;
using LMS.Common.Authorization;
using LMS.Courses.Api.Models;
using LMS.Courses.Application.Models;
using LMS.Courses.Application.Queries.GetCourses;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LMS.Courses.Api.Endpoints;

public static class GetCoursesEndpoint
{
    public static RouteGroupBuilder MapGetCourses(this RouteGroupBuilder group)
    {
        group.MapGet("/", GetCourses)
            .WithName(nameof(GetCourses));

        return group;
    }

    private static async Task<IResult> GetCourses(
        ICurrentUserService currentUserService,
        IQueryHandler<GetCoursesQuery, List<Course>> handler)
    {
        if (currentUserService.UserId is not { } userId)
        {
            return Results.Unauthorized();
        }

        var result = await handler.Handle(new GetCoursesQuery(userId));

        if (result.IsFailure)
        {
            return CourseEndpointResults.FromError(result.Error);
        }

        var response = result.Value
            .Select(course => new CourseResponse(
                course.Id,
                course.AuthorId,
                course.Title,
                course.Theme,
                course.Description,
                course.CreatedAt,
                course.UpdatedAt))
            .ToList();

        return Results.Ok(response);
    }
}
