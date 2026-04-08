using LMS.Courses.Api.Endpoints;
using Microsoft.AspNetCore.Builder;

namespace LMS.Courses.Api.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication UseCoursesModule(this WebApplication app)
    {
        app.MapCourseEndpoints();

        return app;
    }
}