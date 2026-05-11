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
        ICommandHandler<ArchiveLessonCommand, ArchiveLessonResult> handler)
    {
        var result = await handler.HandleAsync(new ArchiveLessonCommand(courseId, moduleId, lessonId));

        return result.Status switch
        {
            ArchiveLessonStatus.CourseNotFound => Results.NotFound(result.Errors),
            ArchiveLessonStatus.ModuleNotFound => Results.NotFound(result.Errors),
            ArchiveLessonStatus.LessonNotFound => Results.NotFound(result.Errors),
            ArchiveLessonStatus.Success => Results.NoContent(),
            _ => Results.Problem("Unexpected error while archiving lesson.")
        };
    }
}
