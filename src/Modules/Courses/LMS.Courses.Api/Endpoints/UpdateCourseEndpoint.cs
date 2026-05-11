using FluentValidation;
using LMS.Common.CQRS;
using LMS.Courses.Api.Models;
using LMS.Courses.Application.Commands.UpdateCourse;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LMS.Courses.Api.Endpoints;

public static class UpdateCourseEndpoint
{
    public static RouteGroupBuilder MapUpdateCourse(this RouteGroupBuilder group)
    {
        group.MapPut("/{id:guid}", UpdateCourse)
             .WithName(nameof(UpdateCourse));

        return group;
    }

    private static async Task<IResult> UpdateCourse(
        Guid id,
        UpdateCourseRequest request,
        IValidator<UpdateCourseRequest> validator,
        ICommandHandler<UpdateCourseCommand, UpdateCourseResult> handler)
    {
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            return Results.BadRequest(validationResult.Errors);
        }

        var result = await handler.HandleAsync(new UpdateCourseCommand(id, request.Title, request.Theme, request.Description));

        return result.Status switch
        {
            UpdateCourseStatus.NotFound => Results.NotFound(result.Errors),
            UpdateCourseStatus.Success => Results.Ok(
                new CourseResponse(
                    result.Course!.Id,
                    result.Course.AuthorId,
                    result.Course.Title,
                    result.Course.Theme,
                    result.Course.Description,
                    result.Course.CreatedAt,
                    result.Course.UpdatedAt)),
            _ => Results.Problem("Unexpected error while updating course.")
        };
    }
}