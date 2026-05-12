using LMS.Common.CQRS;
using LMS.Courses.Application.Commands.AddCourseMember;
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
using LMS.Courses.Application.Queries.GetCourseMembers;
using LMS.Courses.Application.Queries.GetCourses;
using LMS.Courses.Application.Queries.GetLesson;
using LMS.Courses.Application.Queries.GetLessons;
using Microsoft.Extensions.DependencyInjection;

namespace LMS.Courses.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<CreateCourseCommand, Course>, CreateCourseCommandHandler>();
        services.AddScoped<ICommandHandler<AddCourseMemberCommand, CourseMember>, AddCourseMemberCommandHandler>();
        services.AddScoped<ICommandHandler<CreateCourseModuleCommand, CourseModule>, CreateCourseModuleCommandHandler>();
        services.AddScoped<ICommandHandler<CreateLessonCommand, Lesson>, CreateLessonCommandHandler>();
        services.AddScoped<ICommandHandler<ArchiveCourseModuleCommand>, ArchiveCourseModuleCommandHandler>();
        services.AddScoped<ICommandHandler<ArchiveLessonCommand>, ArchiveLessonCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateCourseCommand, Course>, UpdateCourseCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateCourseModuleCommand, CourseModule>, UpdateCourseModuleCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateLessonCommand, Lesson>, UpdateLessonCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteCourseCommand>, DeleteCourseCommandHandler>();

        services.AddScoped<IQueryHandler<GetCourseQuery, Course>, GetCourseQueryHandler>();
        services.AddScoped<IQueryHandler<GetCourseModuleQuery, CourseModule>, GetCourseModuleQueryHandler>();
        services.AddScoped<IQueryHandler<GetCourseModulesQuery, List<CourseModuleSummary>>, GetCourseModulesQueryHandler>();
        services.AddScoped<IQueryHandler<GetCourseMembersQuery, List<CourseMember>>, GetCourseMembersQueryHandler>();
        services.AddScoped<IQueryHandler<GetCoursesQuery, List<Course>>, GetCoursesQueryHandler>();
        services.AddScoped<IQueryHandler<GetLessonQuery, Lesson>, GetLessonQueryHandler>();
        services.AddScoped<IQueryHandler<GetLessonsQuery, List<LessonSummary>>, GetLessonsQueryHandler>();

        return services;
    }
}
