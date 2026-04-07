using LMS.Users.Api.Endpoints.Auth;
using LMS.Users.Api.Endpoints.UsersOperations;
using Microsoft.AspNetCore.Routing;

namespace LMS.Users.Api.Endpoints;

public static class UsersEndpoints
{
    public static IEndpointRouteBuilder MapUsersEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapUserOperationsEndpoints()
           .MapAuthEndpoints();
        
        return app;
    }
}