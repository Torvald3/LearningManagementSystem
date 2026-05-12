using LMS.Common.CQRS;
using LMS.Users.Api.Models;
using LMS.Users.Application.Queries;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using UserModel = LMS.Users.Application.Models.User;

namespace LMS.Users.Api.Endpoints;

public static class GetUserEndpoint
{
    public static RouteGroupBuilder MapGetUser(this RouteGroupBuilder group)
    {
        group.MapGet("/{userId:guid}", GetUser)
             .WithName(nameof(GetUser));

        return group;
    }

    private static async Task<IResult> GetUser(
        Guid userId,
        IQueryHandler<GetUserQuery, UserModel> handler)
    {
        var result = await handler.Handle(new GetUserQuery(userId));

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
