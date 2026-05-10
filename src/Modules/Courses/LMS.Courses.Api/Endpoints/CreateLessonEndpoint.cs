using FluentValidation;
using LMS.Common.CQRS;
using LMS.Courses.Api.Models;
using LMS.Courses.Application.Commands.CreateLesson;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LMS.Courses.Api.Endpoints;

public static class CreateLessonEndpoint
{
    public static RouteGroupBuilder MapCreateLesson(this RouteGroupBuilder group)
    {
        group.MapPost("/{courseId:guid}/modules/{moduleId:guid}/lessons", CreateLesson)
             .WithName(nameof(CreateLesson));

        return group;
    }

    private static async Task<IResult> CreateLesson(
        Guid courseId,
        Guid moduleId,
        CreateLessonRequest request,
        IValidator<CreateLessonRequest> validator,
        ICommandHandler<CreateLessonCommand, CreateLessonResult> handler)
    {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return Results.BadRequest(validationResult.Errors);
        }

        var result = await handler.HandleAsync(
            new CreateLessonCommand(
                courseId,
                moduleId,
                request.Title,
                request.Content));

        return result.Status switch
        {
            CreateLessonStatus.CourseNotFound => Results.NotFound(result.Errors),
            CreateLessonStatus.ModuleNotFound => Results.NotFound(result.Errors),
            CreateLessonStatus.Success => Results.Created(
                $"/api/courses/{courseId}/modules/{moduleId}/lessons/{result.Lesson!.Id}",
                new LessonResponse(
                    result.Lesson.Id,
                    result.Lesson.ModuleId,
                    result.Lesson.Title,
                    result.Lesson.Content,
                    result.Lesson.Position,
                    result.Lesson.CreatedAt,
                    result.Lesson.UpdatedAt)),
            _ => Results.Problem("Unexpected error while creating lesson.")
        };
    }
}
