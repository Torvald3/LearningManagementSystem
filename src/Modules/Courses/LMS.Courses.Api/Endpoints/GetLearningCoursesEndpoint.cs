using LMS.Common.Authorization;
using LMS.Common.CQRS;
using LMS.Courses.Api.Models;
using LMS.Courses.Application.Models;
using LMS.Courses.Application.Queries.GetLearningCourses;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LMS.Courses.Api.Endpoints;

public static class GetLearningCoursesEndpoint
{
    public static RouteGroupBuilder MapGetLearningCourses(this RouteGroupBuilder group)
    {
        group.MapGet("/my/learning", GetLearningCourses)
             .WithName(nameof(GetLearningCourses));

        return group;
    }

    private static async Task<IResult> GetLearningCourses(
        ICurrentUserService currentUserService,
        IQueryHandler<GetLearningCoursesQuery, List<Course>> handler)
    {
        if (currentUserService.UserId is not { } userId)
        {
            return Results.Unauthorized();
        }

        var result = await handler.Handle(new GetLearningCoursesQuery(userId));

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
