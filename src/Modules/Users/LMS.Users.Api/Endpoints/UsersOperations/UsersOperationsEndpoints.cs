using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LMS.Users.Api.Endpoints.UsersOperations;

internal static class UsersOperationsEndpoints
{
    public static IEndpointRouteBuilder MapUserOperationsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/users")
            .WithTags("Users");
        
        //app.Map...
        
        return app;
    }
}