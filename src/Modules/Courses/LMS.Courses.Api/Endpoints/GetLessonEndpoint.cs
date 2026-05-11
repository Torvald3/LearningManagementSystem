using LMS.Common.CQRS;
using LMS.Courses.Api.Models;
using LMS.Courses.Application.Models;
using LMS.Courses.Application.Queries.GetLesson;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LMS.Courses.Api.Endpoints;

public static class GetLessonEndpoint
{
    public static RouteGroupBuilder MapGetLesson(this RouteGroupBuilder group)
    {
        group.MapGet("/{courseId:guid}/modules/{moduleId:guid}/lessons/{lessonId:guid}", GetLesson)
             .WithName(nameof(GetLesson));

        return group;
    }

    private static async Task<IResult> GetLesson(
        Guid courseId,
        Guid moduleId,
        Guid lessonId,
        IQueryHandler<GetLessonQuery, Lesson> handler)
    {
        var result = await handler.Handle(new GetLessonQuery(courseId, moduleId, lessonId));

        if (result.IsFailure)
        {
            return Results.NotFound();
        }

        var lesson = result.Value;

        return Results.Ok(new LessonResponse(
            lesson.Id,
            lesson.ModuleId,
            lesson.Title,
            lesson.Content,
            lesson.Position,
            lesson.CreatedAt,
            lesson.UpdatedAt));
    }
}
