using LMS.Common.CQRS;
using LMS.Courses.Application.Commands.CreateCourse;
using LMS.Courses.Application.Commands.DeleteCourse;
using LMS.Courses.Application.Commands.UpdateCourse;
using LMS.Courses.Application.Models;
using LMS.Courses.Application.Queries.GetCourse;
using LMS.Courses.Application.Queries.GetCourses;
using Microsoft.Extensions.DependencyInjection;

namespace LMS.Courses.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<CreateCourseCommand, CreateCourseResult>, CreateCourseCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateCourseCommand, UpdateCourseResult>, UpdateCourseCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteCourseCommand, bool>, DeleteCourseCommandHandler>();

        services.AddScoped<IQueryHandler<GetCourseQuery, Course?>, GetCourseQueryHandler>();
        services.AddScoped<IQueryHandler<GetCoursesQuery, IReadOnlyList<Course>>, GetCoursesQueryHandler>();
        
        return services;
    }
}