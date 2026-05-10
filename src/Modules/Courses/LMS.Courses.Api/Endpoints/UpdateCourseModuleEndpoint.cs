using FluentValidation;
using LMS.Common.CQRS;
using LMS.Courses.Api.Models;
using LMS.Courses.Application.Commands.UpdateCourseModule;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LMS.Courses.Api.Endpoints;

public static class UpdateCourseModuleEndpoint
{
    public static RouteGroupBuilder MapUpdateCourseModule(this RouteGroupBuilder group)
    {
        group.MapPut("/{courseId:guid}/modules/{moduleId:guid}", UpdateCourseModule)
             .WithName(nameof(UpdateCourseModule));

        return group;
    }

    private static async Task<IResult> UpdateCourseModule(
        Guid courseId,
        Guid moduleId,
        UpdateCourseModuleRequest request,
        IValidator<UpdateCourseModuleRequest> validator,
        ICommandHandler<UpdateCourseModuleCommand, UpdateCourseModuleResult> handler)
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

        return result.Status switch
        {
            UpdateCourseModuleStatus.CourseNotFound => Results.NotFound(result.Errors),
            UpdateCourseModuleStatus.ModuleNotFound => Results.NotFound(result.Errors),
            UpdateCourseModuleStatus.InvalidPosition => Results.BadRequest(result.Errors),
            UpdateCourseModuleStatus.Success => Results.Ok(
                new CourseModuleResponse(
                    result.Module!.Id,
                    result.Module.CourseId,
                    result.Module.Title,
                    result.Module.Description,
                    result.Module.Position,
                    result.Module.CreatedAt,
                    result.Module.UpdatedAt)),
            _ => Results.Problem("Unexpected error while updating course module.")
        };
    }
}
