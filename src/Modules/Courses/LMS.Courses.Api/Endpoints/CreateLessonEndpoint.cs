using FluentValidation;
using LMS.Common.CQRS;
using LMS.Courses.Api.Authorization;
using LMS.Courses.Api.Models;
using LMS.Courses.Application.Commands.CreateLesson;
using LMS.Courses.Application.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LMS.Courses.Api.Endpoints;

public static class CreateLessonEndpoint
{
    public static RouteGroupBuilder MapCreateLesson(this RouteGroupBuilder group)
    {
        group.MapPost("/{courseId:guid}/modules/{moduleId:guid}/lessons", CreateLesson)
             .WithName(nameof(CreateLesson))
             .RequireAuthorization(CourseAuthorizationPolicies.CourseEditor);

        return group;
    }

    private static async Task<IResult> CreateLesson(
        Guid courseId,
        Guid moduleId,
        CreateLessonRequest request,
        IValidator<CreateLessonRequest> validator,
        ICommandHandler<CreateLessonCommand, Lesson> handler)
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

        if (result.IsFailure)
        {
            return CourseEndpointResults.FromError(result.Error);
        }

        var lesson = result.Value;

        return Results.Created(
            $"/api/courses/{courseId}/modules/{moduleId}/lessons/{lesson.Id}",
            new LessonResponse(
                lesson.Id,
                lesson.ModuleId,
                lesson.Title,
                lesson.Content,
                lesson.Position,
                lesson.CreatedAt,
                lesson.UpdatedAt));
    }
}
