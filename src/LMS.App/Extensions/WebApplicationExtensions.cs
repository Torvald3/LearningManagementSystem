using LMS.Courses.Api.Extensions;
using LMS.Identity.Api.Endpoints;
using LMS.Users.Api.Extensions;

namespace LMS.App.Extensions;

public static class ApplicationBuilderExtensions
{
    public static WebApplication UseModules(this WebApplication app)
    {
        app.MapIdentityEndpoints();
        
        app.UseUsersModule();
        app.UseCoursesModule();

        return app;
    }
}