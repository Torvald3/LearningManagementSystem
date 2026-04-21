using LMS.Courses.Api.Extensions;
using LMS.Identity.Api.Endpoints;
using LMS.Identity.Api.Extensions;
using LMS.Users.Api.Extensions;

namespace LMS.App.Extensions;

public static class ApplicationBuilderExtensions
{
    public static WebApplication UseModules(this WebApplication app)
    {
        app.UseUsersModule()
           .UseIdentityModule()
           .UseCoursesModule();

        app.MapMetricsEndpoints();

        return app;
    }
}