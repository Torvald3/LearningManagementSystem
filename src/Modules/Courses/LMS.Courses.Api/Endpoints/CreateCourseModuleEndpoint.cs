using FluentValidation;
using LMS.Common.CQRS;
using LMS.Courses.Api.Models;
using LMS.Courses.Application.Commands.CreateCourseModule;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LMS.Courses.Api.Endpoints;

public static class CreateCourseModuleEndpoint
{
    public static RouteGroupBuilder MapCreateCourseModule(this RouteGroupBuilder group)
    {
        group.MapPost("/{courseId:guid}/modules", CreateCourseModule)
             .WithName(nameof(CreateCourseModule));

        return group;
    }

    private static async Task<IResult> CreateCourseModule(
        Guid courseId,
        CreateCourseModuleRequest request,
        IValidator<CreateCourseModuleRequest> validator,
        ICommandHandler<CreateCourseModuleCommand, CreateCourseModuleResult> handler)
    {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return Results.BadRequest(validationResult.Errors);
        }

        var result = await handler.HandleAsync(
            new CreateCourseModuleCommand(
                courseId,
                request.Title,
                request.Description));

        return result.Status switch
        {
            CreateCourseModuleStatus.CourseNotFound => Results.NotFound(result.Errors),
            CreateCourseModuleStatus.Success => Results.Created(
                $"/api/courses/{courseId}/modules/{result.Module!.Id}",
                new CourseModuleResponse(
                    result.Module.Id,
                    result.Module.CourseId,
                    result.Module.Title,
                    result.Module.Description,
                    result.Module.Position,
                    result.Module.CreatedAt,
                    result.Module.UpdatedAt)),
            _ => Results.Problem("Unexpected error while creating course module.")
        };
    }
}
