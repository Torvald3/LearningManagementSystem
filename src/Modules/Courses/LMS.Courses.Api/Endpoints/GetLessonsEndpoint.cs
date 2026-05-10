using LMS.Common.CQRS;
using LMS.Courses.Api.Models;
using LMS.Courses.Application.Models;
using LMS.Courses.Application.Queries.GetLessons;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LMS.Courses.Api.Endpoints;

public static class GetLessonsEndpoint
{
    public static RouteGroupBuilder MapGetLessons(this RouteGroupBuilder group)
    {
        group.MapGet("/{courseId:guid}/modules/{moduleId:guid}/lessons", GetLessons)
             .WithName(nameof(GetLessons));

        return group;
    }

    private static async Task<IResult> GetLessons(
        Guid courseId,
        Guid moduleId,
        IQueryHandler<GetLessonsQuery, IReadOnlyList<LessonSummary>?> handler)
    {
        var lessons = await handler.Handle(new GetLessonsQuery(courseId, moduleId));

        if (lessons is null)
        {
            return Results.NotFound();
        }

        var response = lessons
            .Select(lesson => new LessonSummaryResponse(
                lesson.Id,
                lesson.ModuleId,
                lesson.Title,
                lesson.Position))
            .ToList();

        return Results.Ok(response);
    }
}
