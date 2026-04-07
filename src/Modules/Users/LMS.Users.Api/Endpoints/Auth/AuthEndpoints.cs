using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LMS.Users.Api.Endpoints.Auth;

internal static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth")
            .WithTags("Auth");

        group.MapRegisterUser()
             .MapConfirmEmail()
             .MapLoginEndpoint();

        group.MapGet("/test", () => "It's just test endpoint")
               .RequireAuthorization();
        
        return app;
    }
}