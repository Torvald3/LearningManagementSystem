using LMS.Common.Authorization;
using LMS.Common.CQRS;
using LMS.Courses.Api.Models;
using LMS.Courses.Application.Models;
using LMS.Courses.Application.Queries.GetTeachingCourses;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LMS.Courses.Api.Endpoints;

public static class GetTeachingCoursesEndpoint
{
    public static RouteGroupBuilder MapGetTeachingCourses(this RouteGroupBuilder group)
    {
        group.MapGet("/my/teaching", GetTeachingCourses)
             .WithName(nameof(GetTeachingCourses));

        return group;
    }

    private static async Task<IResult> GetTeachingCourses(
        ICurrentUserService currentUserService,
        IQueryHandler<GetTeachingCoursesQuery, List<Course>> handler)
    {
        if (currentUserService.UserId is not { } userId)
        {
            return Results.Unauthorized();
        }

        var result = await handler.Handle(new GetTeachingCoursesQuery(userId));

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
