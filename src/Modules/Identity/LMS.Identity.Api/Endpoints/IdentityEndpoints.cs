using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LMS.Identity.Api.Endpoints;

public static class IdentityEndpoints
{
    public static IEndpointRouteBuilder MapIdentityEndpoints(this IEndpointRouteBuilder app)
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