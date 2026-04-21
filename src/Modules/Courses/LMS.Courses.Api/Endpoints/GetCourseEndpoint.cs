using LMS.Common.CQRS;
using LMS.Courses.Api.Models;
using LMS.Courses.Application.Models;
using LMS.Courses.Application.Queries.GetCourse;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LMS.Courses.Api.Endpoints;

public static class GetCourseEndpoint
{
    public static RouteGroupBuilder MapGetCourse(this RouteGroupBuilder group)
    {
        group.MapGet("/{id:guid}", GetCourse)
             .WithName(nameof(GetCourse));

        return group;
    }

    private static async Task<IResult> GetCourse(
        Guid id,
        IQueryHandler<GetCourseQuery, Course?> handler)
    {
        var course = await handler.Handle(new GetCourseQuery(id));

        if (course is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(new CourseResponse(
            course.Id,
            course.AuthorId,
            course.Title,
            course.Theme,
            course.Description,
            course.CreatedAt,
            course.UpdatedAt));
    }
}