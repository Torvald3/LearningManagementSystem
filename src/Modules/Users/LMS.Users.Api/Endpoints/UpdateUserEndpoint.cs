using FluentValidation;
using LMS.Common.Authorization;
using LMS.Common.CQRS;
using LMS.Users.Api.Models;
using LMS.Users.Application.Commands;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using UserModel = LMS.Users.Application.Models.User;

namespace LMS.Users.Api.Endpoints;

public static class UpdateUserEndpoint
{
    public static RouteGroupBuilder MapUpdateUser(this RouteGroupBuilder group)
    {
        group.MapPut("/{userId:guid}", UpdateUser)
             .WithName(nameof(UpdateUser));

        return group;
    }

    private static async Task<IResult> UpdateUser(
        Guid userId,
        UpdateUserRequest request,
        IValidator<UpdateUserRequest> validator,
        ICurrentUserService currentUserService,
        ICommandHandler<UpdateUserCommand, UserModel> handler)
    {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return Results.BadRequest(validationResult.Errors);
        }

        if (currentUserService.UserId != userId)
        {
            return Results.Forbid();
        }

        var result = await handler.HandleAsync(
            new UpdateUserCommand(
                userId,
                request.Bio,
                request.AvatarMediaId));

        if (result.IsFailure)
        {
            return UsersEndpointResults.FromError(result.Error);
        }

        var user = result.Value;

        return Results.Ok(new UserResponse(
            user.Id,
            user.Username,
            user.Email,
            user.Bio,
            user.AvatarMediaId));
    }
}
