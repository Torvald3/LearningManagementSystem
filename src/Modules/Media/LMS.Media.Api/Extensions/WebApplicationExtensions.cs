using LMS.Media.Api.Endpoints;
using Microsoft.AspNetCore.Builder;

namespace LMS.Media.Api.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication UseMediaModule(this WebApplication app)
    {
        app.MapMediaEndpoints();

        return app;
    }
}
