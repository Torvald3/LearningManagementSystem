using LMS.Common.CQRS;
using LMS.Courses.Application.Commands.ArchiveCourseModule;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LMS.Courses.Api.Endpoints;

public static class ArchiveCourseModuleEndpoint
{
    public static RouteGroupBuilder MapArchiveCourseModule(this RouteGroupBuilder group)
    {
        group.MapDelete("/{courseId:guid}/modules/{moduleId:guid}", ArchiveCourseModule)
             .WithName(nameof(ArchiveCourseModule));

        return group;
    }

    private static async Task<IResult> ArchiveCourseModule(
        Guid courseId,
        Guid moduleId,
        ICommandHandler<ArchiveCourseModuleCommand, ArchiveCourseModuleResult> handler)
    {
        var result = await handler.HandleAsync(new ArchiveCourseModuleCommand(courseId, moduleId));

        return result.Status switch
        {
            ArchiveCourseModuleStatus.CourseNotFound => Results.NotFound(result.Errors),
            ArchiveCourseModuleStatus.ModuleNotFound => Results.NotFound(result.Errors),
            ArchiveCourseModuleStatus.Success => Results.NoContent(),
            _ => Results.Problem("Unexpected error while archiving course module.")
        };
    }
}
