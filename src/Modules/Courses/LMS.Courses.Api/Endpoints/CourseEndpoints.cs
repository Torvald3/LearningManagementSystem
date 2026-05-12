using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LMS.Courses.Api.Endpoints;

public static class CourseEndpoints
{
    public static IEndpointRouteBuilder MapCourseEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/courses")
            .WithTags("Courses");

        group.MapCreateCourse()
             .MapAddCourseMember()
             .MapCreateCourseModule()
             .MapCreateLesson()
             .MapUpdateCourse()
             .MapUpdateCourseModule()
             .MapUpdateLesson()
             .MapDeleteCourse()
             .MapArchiveCourseModule()
             .MapArchiveLesson()
             .MapGetCourse()
             .MapGetCourseModule()
             .MapGetCourseModules()
             .MapGetCourseMembers()
             .MapGetCourses()
             .MapGetLesson()
             .MapGetLessons();

        return group;
    }
}
