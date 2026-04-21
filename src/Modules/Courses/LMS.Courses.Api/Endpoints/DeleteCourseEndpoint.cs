using LMS.Common.CQRS;
using LMS.Courses.Application.Commands.DeleteCourse;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LMS.Courses.Api.Endpoints;

public static class DeleteCourseEndpoint
{
    public static RouteGroupBuilder MapDeleteCourse(this RouteGroupBuilder group)
    {
        group.MapDelete("/{id:guid}", DeleteCourse)
             .WithName(nameof(DeleteCourse));

        return group;
    }

    private static async Task<IResult> DeleteCourse(
        Guid id,
        ICommandHandler<DeleteCourseCommand, bool> handler)
    {
        var result = await handler.HandleAsync(new DeleteCourseCommand(id));

        if (!result)
        {
            return Results.NotFound();
        }

        return Results.NoContent();
    }
}