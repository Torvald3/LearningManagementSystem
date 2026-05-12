using FluentValidation;
using LMS.Common.CQRS;
using LMS.Courses.Api.Authorization;
using LMS.Courses.Api.Models;
using LMS.Courses.Application.Commands.UpdateCourseModule;
using LMS.Courses.Application.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LMS.Courses.Api.Endpoints;

public static class UpdateCourseModuleEndpoint
{
    public static RouteGroupBuilder MapUpdateCourseModule(this RouteGroupBuilder group)
    {
        group.MapPut("/{courseId:guid}/modules/{moduleId:guid}", UpdateCourseModule)
             .WithName(nameof(UpdateCourseModule))
             .RequireAuthorization(CourseAuthorizationPolicies.CourseEditor);

        return group;
    }

    private static async Task<IResult> UpdateCourseModule(
        Guid courseId,
        Guid moduleId,
        UpdateCourseModuleRequest request,
        IValidator<UpdateCourseModuleRequest> validator,
        ICommandHandler<UpdateCourseModuleCommand, CourseModule> handler)
    {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return Results.BadRequest(validationResult.Errors);
        }

        var result = await handler.HandleAsync(
            new UpdateCourseModuleCommand(
                courseId,
                moduleId,
                request.Title,
                request.Description,
                request.Position));

        if (result.IsFailure)
        {
            return CourseEndpointResults.FromError(result.Error);
        }

        var module = result.Value;

        return Results.Ok(
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
