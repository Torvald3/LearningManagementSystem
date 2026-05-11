using LMS.Common.CQRS;
using LMS.Courses.Application.Commands.ArchiveLesson;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LMS.Courses.Api.Endpoints;

public static class ArchiveLessonEndpoint
{
    public static RouteGroupBuilder MapArchiveLesson(this RouteGroupBuilder group)
    {
        group.MapDelete("/{courseId:guid}/modules/{moduleId:guid}/lessons/{lessonId:guid}", ArchiveLesson)
             .WithName(nameof(ArchiveLesson));

        return group;
    }

    private static async Task<IResult> ArchiveLesson(
        Guid courseId,
        Guid moduleId,
        Guid lessonId,
        ICommandHandler<ArchiveLessonCommand> handler)
    {
        var result = await handler.HandleAsync(new ArchiveLessonCommand(courseId, moduleId, lessonId));

        return result.IsSuccess
            ? Results.NoContent()
            : CourseEndpointResults.FromError(result.Error);
    }
}
