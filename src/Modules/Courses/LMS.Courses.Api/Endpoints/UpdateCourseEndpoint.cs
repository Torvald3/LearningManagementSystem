using FluentValidation;
using LMS.Common.CQRS;
using LMS.Courses.Api.Authorization;
using LMS.Courses.Api.Models;
using LMS.Courses.Application.Commands.UpdateCourse;
using LMS.Courses.Application.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LMS.Courses.Api.Endpoints;

public static class UpdateCourseEndpoint
{
    public static RouteGroupBuilder MapUpdateCourse(this RouteGroupBuilder group)
    {
        group.MapPut("/{id:guid}", UpdateCourse)
             .WithName(nameof(UpdateCourse))
             .RequireAuthorization(CourseAuthorizationPolicies.CourseOwner);

        return group;
    }

    private static async Task<IResult> UpdateCourse(
        Guid id,
        UpdateCourseRequest request,
        IValidator<UpdateCourseRequest> validator,
        ICommandHandler<UpdateCourseCommand, Course> handler)
    {
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            return Results.BadRequest(validationResult.Errors);
        }

        var result = await handler.HandleAsync(new UpdateCourseCommand(id, request.Title, request.Theme, request.Description));

        if (result.IsFailure)
        {
            return CourseEndpointResults.FromError(result.Error);
        }

        var course = result.Value;

        return Results.Ok(
            new CourseResponse(
                course.Id,
                course.Title,
                course.Theme,
                course.Description,
                course.CreatedAt,
                course.UpdatedAt));
    }
}
