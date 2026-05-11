using FluentValidation;
using LMS.Common.CQRS;
using LMS.Courses.Api.Models;
using LMS.Courses.Application.Commands.CreateCourse;
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
        ICommandHandler<CreateCourseCommand, CreateCourseResult> handler)
    {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return Results.BadRequest(validationResult.Errors);
        }

        var result = await handler.HandleAsync(
            new CreateCourseCommand(
                request.AuthorId,
                request.Title,
                request.Theme,
                request.Description));

        return result.Status switch
        {
            CreateCourseStatus.AuthorNotFound => Results.BadRequest(result.Errors),
            CreateCourseStatus.Success => Results.Created(
                $"/api/courses/{result.Course!.Id}",
                new CourseResponse( 
                    result.Course.Id, 
                    result.Course.AuthorId,
                    result.Course.Title,
                    result.Course.Theme,
                    result.Course.Description,
                    result.Course.CreatedAt,
                    result.Course.UpdatedAt)),
            _ => Results.Problem("Unexpected error while creating course.")
        };
    }
}