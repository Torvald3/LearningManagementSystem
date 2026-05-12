using FluentValidation;
using LMS.Common.CQRS;
using LMS.Courses.Api.Authorization;
using LMS.Courses.Api.Models;
using LMS.Courses.Application.Commands.UpdateLesson;
using LMS.Courses.Application.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LMS.Courses.Api.Endpoints;

public static class UpdateLessonEndpoint
{
    public static RouteGroupBuilder MapUpdateLesson(this RouteGroupBuilder group)
    {
        group.MapPut("/{courseId:guid}/modules/{moduleId:guid}/lessons/{lessonId:guid}", UpdateLesson)
             .WithName(nameof(UpdateLesson))
             .RequireAuthorization(CourseAuthorizationPolicies.CourseEditor);

        return group;
    }

    private static async Task<IResult> UpdateLesson(
        Guid courseId,
        Guid moduleId,
        Guid lessonId,
        UpdateLessonRequest request,
        IValidator<UpdateLessonRequest> validator,
        ICommandHandler<UpdateLessonCommand, Lesson> handler)
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

        if (result.IsFailure)
        {
            return CourseEndpointResults.FromError(result.Error);
        }

        var lesson = result.Value;

        return Results.Ok(
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
