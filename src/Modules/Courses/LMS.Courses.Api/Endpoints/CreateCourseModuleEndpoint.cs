using FluentValidation;
using LMS.Common.CQRS;
using LMS.Courses.Api.Authorization;
using LMS.Courses.Api.Models;
using LMS.Courses.Application.Commands.CreateCourseModule;
using LMS.Courses.Application.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LMS.Courses.Api.Endpoints;

public static class CreateCourseModuleEndpoint
{
    public static RouteGroupBuilder MapCreateCourseModule(this RouteGroupBuilder group)
    {
        group.MapPost("/{courseId:guid}/modules", CreateCourseModule)
             .WithName(nameof(CreateCourseModule))
             .RequireAuthorization(CourseAuthorizationPolicies.CourseEditor);

        return group;
    }

    private static async Task<IResult> CreateCourseModule(
        Guid courseId,
        CreateCourseModuleRequest request,
        IValidator<CreateCourseModuleRequest> validator,
        ICommandHandler<CreateCourseModuleCommand, CourseModule> handler)
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

        if (result.IsFailure)
        {
            return CourseEndpointResults.FromError(result.Error);
        }

        var module = result.Value;

        return Results.Created(
            $"/api/courses/{courseId}/modules/{module.Id}",
            new CourseModuleResponse(
                module.Id,
                module.CourseId,
                module.Title,
                module.Description,
                module.Position,
                module.CreatedAt,
                module.UpdatedAt));
    }
}
