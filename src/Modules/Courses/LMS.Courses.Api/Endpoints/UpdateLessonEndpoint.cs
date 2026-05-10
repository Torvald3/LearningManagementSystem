using FluentValidation;
using LMS.Common.CQRS;
using LMS.Courses.Api.Models;
using LMS.Courses.Application.Commands.UpdateLesson;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LMS.Courses.Api.Endpoints;

public static class UpdateLessonEndpoint
{
    public static RouteGroupBuilder MapUpdateLesson(this RouteGroupBuilder group)
    {
        group.MapPut("/{courseId:guid}/modules/{moduleId:guid}/lessons/{lessonId:guid}", UpdateLesson)
             .WithName(nameof(UpdateLesson));

        return group;
    }

    private static async Task<IResult> UpdateLesson(
        Guid courseId,
        Guid moduleId,
        Guid lessonId,
        UpdateLessonRequest request,
        IValidator<UpdateLessonRequest> validator,
        ICommandHandler<UpdateLessonCommand, UpdateLessonResult> handler)
    {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return Results.BadRequest(validationResult.Errors);
        }

        var result = await handler.HandleAsync(
            new UpdateLessonCommand(
                courseId,
                moduleId,
                lessonId,
                request.Title,
                request.Content,
                request.Position));

        return result.Status switch
        {
            UpdateLessonStatus.CourseNotFound => Results.NotFound(result.Errors),
            UpdateLessonStatus.ModuleNotFound => Results.NotFound(result.Errors),
            UpdateLessonStatus.LessonNotFound => Results.NotFound(result.Errors),
            UpdateLessonStatus.InvalidPosition => Results.BadRequest(result.Errors),
            UpdateLessonStatus.Success => Results.Ok(
                new LessonResponse(
                    result.Lesson!.Id,
                    result.Lesson.ModuleId,
                    result.Lesson.Title,
                    result.Lesson.Content,
                    result.Lesson.Position,
                    result.Lesson.CreatedAt,
                    result.Lesson.UpdatedAt)),
            _ => Results.Problem("Unexpected error while updating lesson.")
        };
    }
}
