using FluentValidation;
using LMS.Common.Database.Configuration;
using LMS.Courses.Api.Models;
using LMS.Courses.Api.Validators;
using LMS.Courses.Application.Extensions;
using LMS.Courses.Infrastructure.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LMS.Courses.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoursesModuleServices(this IServiceCollection services, IConfiguration configuration)
    {
        var databaseConfiguration = configuration.GetSection("Database").Get<DatabaseConfiguration>();

        services.AddApiServices()
                .AddApplicationServices()
                .AddInfrastructureServices(databaseConfiguration!);

        return services;
    }

    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddScoped<IValidator<AddCourseMemberRequest>, AddCourseMemberRequestValidator>();
        services.AddScoped<IValidator<CreateCourseRequest>, CreateCourseRequestValidator>();
        services.AddScoped<IValidator<CreateCourseModuleRequest>, CreateCourseModuleRequestValidator>();
        services.AddScoped<IValidator<CreateLessonRequest>, CreateLessonRequestValidator>();
        services.AddScoped<IValidator<UpdateCourseRequest>, UpdateCourseRequestValidator>();
        services.AddScoped<IValidator<UpdateCourseModuleRequest>, UpdateCourseModuleRequestValidator>();
        services.AddScoped<IValidator<UpdateLessonRequest>, UpdateLessonRequestValidator>();

        return services;
    }
}
