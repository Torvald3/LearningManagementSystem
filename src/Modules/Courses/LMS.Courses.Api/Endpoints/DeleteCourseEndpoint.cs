using LMS.Common.CQRS;
using LMS.Courses.Api.Authorization;
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
             .WithName(nameof(DeleteCourse))
             .RequireAuthorization(CourseAuthorizationPolicies.CourseOwner);

        return group;
    }

    private static async Task<IResult> DeleteCourse(
        Guid id,
        ICommandHandler<DeleteCourseCommand> handler)
    {
        var result = await handler.HandleAsync(new DeleteCourseCommand(id));

        return result.IsSuccess
            ? Results.NoContent()
            : Results.NotFound();
    }
}
