using LMS.Users.Api.Endpoints;
using Microsoft.AspNetCore.Builder;

namespace LMS.Users.Api.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication UseUsersModule(this WebApplication app)
    {
        app.MapUsersEndpoints();
        
        return app;
    }
}