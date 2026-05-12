using FluentValidation;
using LMS.Common.Authorization;
using LMS.Common.CQRS;
using LMS.Courses.Api.Models;
using LMS.Courses.Application.Commands.CreateCourse;
using LMS.Courses.Application.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LMS.Courses.Api.Endpoints;

public static class CreateCourseEndpoint
{
    public static RouteGroupBuilder MapCreateCourse(this RouteGroupBuilder group)
    {
        group.MapPost("/", CreateCourse)
            .WithName($"{nameof(CreateCourse)}");

        return group;
    }

    private static async Task<IResult> CreateCourse(
        CreateCourseRequest request,
        IValidator<CreateCourseRequest> validator,
        ICurrentUserService currentUserService,
        ICommandHandler<CreateCourseCommand, Course> handler)
    {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return Results.BadRequest(validationResult.Errors);
        }

        if (currentUserService.UserId is not { } userId)
        {
            return Results.Unauthorized();
        }

        var result = await handler.HandleAsync(
            new CreateCourseCommand(
                userId,
                request.Title,
                request.Theme,
                request.Description));

        if (result.IsFailure)
        {
            return CourseEndpointResults.FromError(result.Error);
        }

        var course = result.Value;

        return Results.Created(
            $"/api/courses/{course.Id}",
            new CourseResponse(
                course.Id,
                course.AuthorId,
                course.Title,
                course.Theme,
                course.Description,
                course.CreatedAt,
                course.UpdatedAt));
    }
}
