using LMS.Common.CQRS;
using LMS.Courses.Application.Commands.ArchiveCourseModule;
using LMS.Courses.Application.Commands.ArchiveLesson;
using LMS.Courses.Application.Commands.CreateCourse;
using LMS.Courses.Application.Commands.CreateCourseModule;
using LMS.Courses.Application.Commands.CreateLesson;
using LMS.Courses.Application.Commands.DeleteCourse;
using LMS.Courses.Application.Commands.UpdateCourse;
using LMS.Courses.Application.Commands.UpdateCourseModule;
using LMS.Courses.Application.Commands.UpdateLesson;
using LMS.Courses.Application.Models;
using LMS.Courses.Application.Queries.GetCourse;
using LMS.Courses.Application.Queries.GetCourseModule;
using LMS.Courses.Application.Queries.GetCourseModules;
using LMS.Courses.Application.Queries.GetCourses;
using LMS.Courses.Application.Queries.GetLesson;
using LMS.Courses.Application.Queries.GetLessons;
using Microsoft.Extensions.DependencyInjection;

namespace LMS.Courses.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<CreateCourseCommand, CreateCourseResult>, CreateCourseCommandHandler>();
        services.AddScoped<ICommandHandler<CreateCourseModuleCommand, CreateCourseModuleResult>, CreateCourseModuleCommandHandler>();
        services.AddScoped<ICommandHandler<CreateLessonCommand, CreateLessonResult>, CreateLessonCommandHandler>();
        services.AddScoped<ICommandHandler<ArchiveCourseModuleCommand, ArchiveCourseModuleResult>, ArchiveCourseModuleCommandHandler>();
        services.AddScoped<ICommandHandler<ArchiveLessonCommand, ArchiveLessonResult>, ArchiveLessonCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateCourseCommand, UpdateCourseResult>, UpdateCourseCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateCourseModuleCommand, UpdateCourseModuleResult>, UpdateCourseModuleCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateLessonCommand, UpdateLessonResult>, UpdateLessonCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteCourseCommand, bool>, DeleteCourseCommandHandler>();

        services.AddScoped<IQueryHandler<GetCourseQuery, Course?>, GetCourseQueryHandler>();
        services.AddScoped<IQueryHandler<GetCourseModuleQuery, CourseModule?>, GetCourseModuleQueryHandler>();
        services.AddScoped<IQueryHandler<GetCourseModulesQuery, IReadOnlyList<CourseModuleSummary>?>, GetCourseModulesQueryHandler>();
        services.AddScoped<IQueryHandler<GetCoursesQuery, IReadOnlyList<Course>>, GetCoursesQueryHandler>();
        services.AddScoped<IQueryHandler<GetLessonQuery, Lesson?>, GetLessonQueryHandler>();
        services.AddScoped<IQueryHandler<GetLessonsQuery, IReadOnlyList<LessonSummary>?>, GetLessonsQueryHandler>();

        return services;
    }
}
