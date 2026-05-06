using LMS.Identity.Api.Endpoints;
using Microsoft.AspNetCore.Builder;

namespace LMS.Identity.Api.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication UseIdentityModule(this WebApplication app)
    {
        app.MapIdentityEndpoints();
        
        return app;
    }
}