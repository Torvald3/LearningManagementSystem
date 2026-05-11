using LMS.Common.CQRS;
using LMS.Common.Observability.Metrics;
using LMS.Courses.Application.Commands.ArchiveCourseModule;
using LMS.Courses.Application.Commands.ArchiveLesson;
using LMS.Courses.Application.Commands.CreateCourse;
using LMS.Courses.Application.Commands.CreateCourseModule;
using LMS.Courses.Application.Commands.CreateLesson;
using LMS.Courses.Application.Commands.DeleteCourse;
using LMS.Courses.Application.Commands.UpdateCourse;
using LMS.Courses.Application.Commands.UpdateCourseModule;
using LMS.Courses.Application.Commands.UpdateLesson;
using LMS.Courses.Application.Queries.GetCourse;
using LMS.Courses.Application.Queries.GetCourseModule;
using LMS.Courses.Application.Queries.GetCourseModules;
using LMS.Courses.Application.Queries.GetCourses;
using LMS.Courses.Application.Queries.GetLesson;
using LMS.Courses.Application.Queries.GetLessons;
using LMS.Courses.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CourseModel = LMS.Courses.Application.Models.Course;
using CourseModuleModel = LMS.Courses.Application.Models.CourseModule;
using CourseModuleSummaryModel = LMS.Courses.Application.Models.CourseModuleSummary;
using LessonModel = LMS.Courses.Application.Models.Lesson;
using LessonSummaryModel = LMS.Courses.Application.Models.LessonSummary;

namespace LMS.Courses.IntegrationTests;

[Collection(CoursesIntegrationTestCollection.Name)]
public sealed class CourseHandlersIntegrationTests : IAsyncLifetime
{
    private readonly CoursesApplicationFixture _fixture;

    public CourseHandlersIntegrationTests(CoursesApplicationFixture fixture)
    {
        _fixture = fixture;
    }

    public Task InitializeAsync() => _fixture.ResetDatabaseAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task CreateCourse_WithExistingAuthor_ShouldPersistCourse_AndUpdateMetrics()
    {
        var authorId = await _fixture.SeedUserAsync();

        var result = await _fixture.ExecuteInScopeAsync(async serviceProvider =>
        {
            var handler = serviceProvider
                .GetRequiredService<ICommandHandler<CreateCourseCommand, CourseModel>>();

            return await handler.HandleAsync(
                new CreateCourseCommand(
                    authorId,
                    "C# Basics",
                    "Programming",
                    "Intro course"));
        });

        Assert.True(result.IsSuccess);
        var course = result.Value;
        Assert.Equal(authorId, course.AuthorId);
        Assert.Equal("C# Basics", course.Title);
        Assert.Equal("Programming", course.Theme);
        Assert.Equal("Intro course", course.Description);

        await _fixture.ExecuteInScopeAsync(async serviceProvider =>
        {
            var coursesDbContext = serviceProvider.GetRequiredService<CoursesDbContext>();
            var metrics = serviceProvider.GetRequiredService<AppMetrics>();

            var courseInDb = await coursesDbContext.Courses
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == course.Id);

            Assert.NotNull(courseInDb);
            Assert.Equal(authorId, courseInDb!.AuthorId);
            Assert.Equal("C# Basics", courseInDb.Title);
            Assert.Equal(1, metrics.CoursesTotal);
        });
    }

    [Fact]
    public async Task CreateCourse_WithUnknownAuthor_ShouldReturnAuthorNotFound_AndNotPersistAnything()
    {
        var result = await _fixture.ExecuteInScopeAsync(async serviceProvider =>
        {
            var handler = serviceProvider
                .GetRequiredService<ICommandHandler<CreateCourseCommand, CourseModel>>();

            return await handler.HandleAsync(
                new CreateCourseCommand(
                    Guid.NewGuid(),
                    "C# Basics",
                    "Programming",
                    "Intro course"));
        });

        Assert.True(result.IsFailure);
        Assert.Equal("courses.author_not_found", result.Error.Code);
        Assert.Contains("does not exist", result.Error.Message);

        await _fixture.ExecuteInScopeAsync(async serviceProvider =>
        {
            var coursesDbContext = serviceProvider.GetRequiredService<CoursesDbContext>();

            var coursesCount = await coursesDbContext.Courses.CountAsync();
            Assert.Equal(0, coursesCount);
        });
    }

    [Fact]
    public async Task CreateCourseModule_WithExistingCourse_ShouldPersistModule()
    {
        var authorId = await _fixture.SeedUserAsync();
        var courseId = await _fixture.SeedCourseAsync(authorId);

        var result = await _fixture.ExecuteInScopeAsync(async serviceProvider =>
        {
            var handler = serviceProvider
                .GetRequiredService<ICommandHandler<CreateCourseModuleCommand, CourseModuleModel>>();

            return await handler.HandleAsync(
                new CreateCourseModuleCommand(
                    courseId,
                    "Getting Started",
                    "Course introduction"));
        });

        Assert.True(result.IsSuccess);
        var module = result.Value;
        Assert.Equal(courseId, module.CourseId);
        Assert.Equal("Getting Started", module.Title);
        Assert.Equal("Course introduction", module.Description);
        Assert.Equal(1, module.Position);

        await _fixture.ExecuteInScopeAsync(async serviceProvider =>
        {
            var coursesDbContext = serviceProvider.GetRequiredService<CoursesDbContext>();

            var moduleInDb = await coursesDbContext.CourseModules
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == module.Id);

            Assert.NotNull(moduleInDb);
            Assert.Equal(courseId, moduleInDb!.CourseId);
            Assert.Equal("Getting Started", moduleInDb.Title);
            Assert.Equal(1, moduleInDb.Position);
        });
    }

    [Fact]
    public async Task CreateCourseModule_WithUnknownCourse_ShouldReturnCourseNotFound_AndNotPersistAnything()
    {
        var result = await _fixture.ExecuteInScopeAsync(async serviceProvider =>
        {
            var handler = serviceProvider
                .GetRequiredService<ICommandHandler<CreateCourseModuleCommand, CourseModuleModel>>();

            return await handler.HandleAsync(
                new CreateCourseModuleCommand(
                    Guid.NewGuid(),
                    "Getting Started",
                    "Course introduction"));
        });

        Assert.True(result.IsFailure);
        Assert.Equal("courses.course_not_found", result.Error.Code);
        Assert.Contains("not found", result.Error.Message);

        await _fixture.ExecuteInScopeAsync(async serviceProvider =>
        {
            var coursesDbContext = serviceProvider.GetRequiredService<CoursesDbContext>();

            var modulesCount = await coursesDbContext.CourseModules.CountAsync();
            Assert.Equal(0, modulesCount);
        });
    }

    [Fact]
    public async Task CreateLesson_WithExistingModule_ShouldPersistLesson()
    {
        var authorId = await _fixture.SeedUserAsync();
        var courseId = await _fixture.SeedCourseAsync(authorId);
        var moduleId = await _fixture.SeedCourseModuleAsync(courseId);

        var result = await _fixture.ExecuteInScopeAsync(async serviceProvider =>
        {
            var handler = serviceProvider
                .GetRequiredService<ICommandHandler<CreateLessonCommand, LessonModel>>();

            return await handler.HandleAsync(
                new CreateLessonCommand(
                    courseId,
                    moduleId,
                    "Variables",
                    "Working with variables"));
        });

        Assert.True(result.IsSuccess);
        var lesson = result.Value;
        Assert.Equal(moduleId, lesson.ModuleId);
        Assert.Equal("Variables", lesson.Title);
        Assert.Equal("Working with variables", lesson.Content);
        Assert.Equal(1, lesson.Position);

        await _fixture.ExecuteInScopeAsync(async serviceProvider =>
        {
            var coursesDbContext = serviceProvider.GetRequiredService<CoursesDbContext>();

            var lessonInDb = await coursesDbContext.Lessons
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == lesson.Id);

            Assert.NotNull(lessonInDb);
            Assert.Equal(moduleId, lessonInDb!.ModuleId);
            Assert.Equal("Variables", lessonInDb.Title);
            Assert.Equal(1, lessonInDb.Position);
        });
    }

    [Fact]
    public async Task CreateLesson_WithUnknownCourse_ShouldReturnCourseNotFound_AndNotPersistAnything()
    {
        var result = await _fixture.ExecuteInScopeAsync(async serviceProvider =>
        {
            var handler = serviceProvider
                .GetRequiredService<ICommandHandler<CreateLessonCommand, LessonModel>>();

            return await handler.HandleAsync(
                new CreateLessonCommand(
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    "Variables",
                    "Working with variables"));
        });

        Assert.True(result.IsFailure);
        Assert.Equal("courses.course_not_found", result.Error.Code);
        Assert.Contains("not found", result.Error.Message);

        await _fixture.ExecuteInScopeAsync(async serviceProvider =>
        {
            var coursesDbContext = serviceProvider.GetRequiredService<CoursesDbContext>();

            var lessonsCount = await coursesDbContext.Lessons.CountAsync();
            Assert.Equal(0, lessonsCount);
        });
    }

    [Fact]
    public async Task CreateLesson_WithUnknownModule_ShouldReturnModuleNotFound_AndNotPersistAnything()
    {
        var authorId = await _fixture.SeedUserAsync();
        var courseId = await _fixture.SeedCourseAsync(authorId);

        var result = await _fixture.ExecuteInScopeAsync(async serviceProvider =>
        {
            var handler = serviceProvider
                .GetRequiredService<ICommandHandler<CreateLessonCommand, LessonModel>>();

            return await handler.HandleAsync(
                new CreateLessonCommand(
                    courseId,
                    Guid.NewGuid(),
                    "Variables",
                    "Working with variables"));
        });

        Assert.True(result.IsFailure);
        Assert.Equal("courses.module_not_found", result.Error.Code);
        Assert.Contains("not found", result.Error.Message);

        await _fixture.ExecuteInScopeAsync(async serviceProvider =>
        {
            var coursesDbContext = serviceProvider.GetRequiredService<CoursesDbContext>();

            var lessonsCount = await coursesDbContext.Lessons.CountAsync();
            Assert.Equal(0, lessonsCount);
        });
    }

    [Fact]
    public async Task GetCourseModule_ShouldReturnExistingModule()
    {
        var authorId = await _fixture.SeedUserAsync();
        var courseId = await _fixture.SeedCourseAsync(authorId);
        var moduleId = await _fixture.SeedCourseModuleAsync(
            courseId,
            "Basics",
            "Start here",
            3);

        var result = await _fixture.ExecuteInScopeAsync(async serviceProvider =>
        {
            var handler = serviceProvider
                .GetRequiredService<IQueryHandler<GetCourseModuleQuery, CourseModuleModel>>();

            return await handler.Handle(new GetCourseModuleQuery(courseId, moduleId));
        });

        Assert.True(result.IsSuccess);
        var module = result.Value;
        Assert.Equal(moduleId, module.Id);
        Assert.Equal(courseId, module.CourseId);
        Assert.Equal("Basics", module.Title);
        Assert.Equal("Start here", module.Description);
        Assert.Equal(3, module.Position);
    }

    [Fact]
    public async Task GetCourseModules_ShouldReturnModulesOrderedByPosition_WithLessonCounts()
    {
        var authorId = await _fixture.SeedUserAsync();
        var courseId = await _fixture.SeedCourseAsync(authorId);

        var secondModuleId = await _fixture.SeedCourseModuleAsync(
            courseId,
            "Second",
            "Second module",
            2);

        var firstModuleId = await _fixture.SeedCourseModuleAsync(
            courseId,
            "First",
            "First module",
            1);

        await _fixture.SeedLessonAsync(firstModuleId, "First lesson", "Full content", 1);
        await _fixture.SeedLessonAsync(firstModuleId, "Second lesson", "More content", 2);
        await _fixture.SeedLessonAsync(secondModuleId, "Only lesson", "Other content", 1);

        var result = await _fixture.ExecuteInScopeAsync(async serviceProvider =>
        {
            var handler = serviceProvider
                .GetRequiredService<IQueryHandler<GetCourseModulesQuery, IReadOnlyList<CourseModuleSummaryModel>>>();

            return await handler.Handle(new GetCourseModulesQuery(courseId));
        });

        Assert.True(result.IsSuccess);
        var modules = result.Value;
        Assert.Equal(2, modules.Count);
        Assert.Equal(firstModuleId, modules[0].Id);
        Assert.Equal(2, modules[0].LessonsCount);
        Assert.Equal(secondModuleId, modules[1].Id);
        Assert.Equal(1, modules[1].LessonsCount);
    }

    [Fact]
    public async Task GetLesson_ShouldReturnExistingLessonWithContent()
    {
        var authorId = await _fixture.SeedUserAsync();
        var courseId = await _fixture.SeedCourseAsync(authorId);
        var moduleId = await _fixture.SeedCourseModuleAsync(courseId);
        var lessonId = await _fixture.SeedLessonAsync(
            moduleId,
            "Variables",
            "Full lesson content",
            5);

        var result = await _fixture.ExecuteInScopeAsync(async serviceProvider =>
        {
            var handler = serviceProvider
                .GetRequiredService<IQueryHandler<GetLessonQuery, LessonModel>>();

            return await handler.Handle(new GetLessonQuery(courseId, moduleId, lessonId));
        });

        Assert.True(result.IsSuccess);
        var lesson = result.Value;
        Assert.Equal(lessonId, lesson.Id);
        Assert.Equal(moduleId, lesson.ModuleId);
        Assert.Equal("Variables", lesson.Title);
        Assert.Equal("Full lesson content", lesson.Content);
        Assert.Equal(5, lesson.Position);
    }

    [Fact]
    public async Task GetLessons_ShouldReturnLessonsOrderedByPosition_WithoutContent()
    {
        var authorId = await _fixture.SeedUserAsync();
        var courseId = await _fixture.SeedCourseAsync(authorId);
        var moduleId = await _fixture.SeedCourseModuleAsync(courseId);

        var secondLessonId = await _fixture.SeedLessonAsync(
            moduleId,
            "Second lesson",
            "Second full content",
            2);

        var firstLessonId = await _fixture.SeedLessonAsync(
            moduleId,
            "First lesson",
            "First full content",
            1);

        var result = await _fixture.ExecuteInScopeAsync(async serviceProvider =>
        {
            var handler = serviceProvider
                .GetRequiredService<IQueryHandler<GetLessonsQuery, IReadOnlyList<LessonSummaryModel>>>();

            return await handler.Handle(new GetLessonsQuery(courseId, moduleId));
        });

        Assert.True(result.IsSuccess);
        var lessons = result.Value;
        Assert.Equal(2, lessons.Count);
        Assert.Equal(firstLessonId, lessons[0].Id);
        Assert.Equal("First lesson", lessons[0].Title);
        Assert.Equal(1, lessons[0].Position);
        Assert.Equal(secondLessonId, lessons[1].Id);
        Assert.Equal("Second lesson", lessons[1].Title);
        Assert.Equal(2, lessons[1].Position);
    }

    [Fact]
    public async Task UpdateCourseModule_ShouldUpdateModule_AndReorderSiblingPositions()
    {
        var authorId = await _fixture.SeedUserAsync();
        var courseId = await _fixture.SeedCourseAsync(authorId);

        var firstModuleId = await _fixture.SeedCourseModuleAsync(courseId, "First", "First module", 1);
        var secondModuleId = await _fixture.SeedCourseModuleAsync(courseId, "Second", "Second module", 2);
        var thirdModuleId = await _fixture.SeedCourseModuleAsync(courseId, "Third", "Third module", 3);

        var result = await _fixture.ExecuteInScopeAsync(async serviceProvider =>
        {
            var handler = serviceProvider
                .GetRequiredService<ICommandHandler<UpdateCourseModuleCommand, CourseModuleModel>>();

            return await handler.HandleAsync(
                new UpdateCourseModuleCommand(
                    courseId,
                    thirdModuleId,
                    "Moved Third",
                    "Moved module",
                    1));
        });

        Assert.True(result.IsSuccess);
        var module = result.Value;
        Assert.Equal(thirdModuleId, module.Id);
        Assert.Equal("Moved Third", module.Title);
        Assert.Equal("Moved module", module.Description);
        Assert.Equal(1, module.Position);

        await _fixture.ExecuteInScopeAsync(async serviceProvider =>
        {
            var coursesDbContext = serviceProvider.GetRequiredService<CoursesDbContext>();

            var modules = await coursesDbContext.CourseModules
                .AsNoTracking()
                .Where(x => x.CourseId == courseId)
                .OrderBy(x => x.Position)
                .ToListAsync();

            Assert.Equal([thirdModuleId, firstModuleId, secondModuleId], modules.Select(x => x.Id).ToArray());
            Assert.Equal([1, 2, 3], modules.Select(x => x.Position).ToArray());
            Assert.Equal("Moved Third", modules[0].Title);
        });
    }

    [Fact]
    public async Task UpdateCourseModule_WithInvalidPosition_ShouldReturnInvalidPosition_AndKeepOrder()
    {
        var authorId = await _fixture.SeedUserAsync();
        var courseId = await _fixture.SeedCourseAsync(authorId);

        var firstModuleId = await _fixture.SeedCourseModuleAsync(courseId, "First", "First module", 1);
        var secondModuleId = await _fixture.SeedCourseModuleAsync(courseId, "Second", "Second module", 2);

        var result = await _fixture.ExecuteInScopeAsync(async serviceProvider =>
        {
            var handler = serviceProvider
                .GetRequiredService<ICommandHandler<UpdateCourseModuleCommand, CourseModuleModel>>();

            return await handler.HandleAsync(
                new UpdateCourseModuleCommand(
                    courseId,
                    secondModuleId,
                    "Invalid Move",
                    "Invalid module",
                    3));
        });

        Assert.True(result.IsFailure);
        Assert.Equal("courses.invalid_position", result.Error.Code);
        Assert.Equal("Position must be between 1 and 2.", result.Error.Message);

        await _fixture.ExecuteInScopeAsync(async serviceProvider =>
        {
            var coursesDbContext = serviceProvider.GetRequiredService<CoursesDbContext>();

            var modules = await coursesDbContext.CourseModules
                .AsNoTracking()
                .Where(x => x.CourseId == courseId)
                .OrderBy(x => x.Position)
                .ToListAsync();

            Assert.Equal([firstModuleId, secondModuleId], modules.Select(x => x.Id).ToArray());
            Assert.Equal(["First", "Second"], modules.Select(x => x.Title).ToArray());
        });
    }

    [Fact]
    public async Task UpdateLesson_ShouldUpdateLesson_AndReorderSiblingPositions()
    {
        var authorId = await _fixture.SeedUserAsync();
        var courseId = await _fixture.SeedCourseAsync(authorId);
        var moduleId = await _fixture.SeedCourseModuleAsync(courseId);

        var firstLessonId = await _fixture.SeedLessonAsync(moduleId, "First", "First content", 1);
        var secondLessonId = await _fixture.SeedLessonAsync(moduleId, "Second", "Second content", 2);
        var thirdLessonId = await _fixture.SeedLessonAsync(moduleId, "Third", "Third content", 3);

        var result = await _fixture.ExecuteInScopeAsync(async serviceProvider =>
        {
            var handler = serviceProvider
                .GetRequiredService<ICommandHandler<UpdateLessonCommand, LessonModel>>();

            return await handler.HandleAsync(
                new UpdateLessonCommand(
                    courseId,
                    moduleId,
                    thirdLessonId,
                    "Moved Third",
                    "Moved content",
                    1));
        });

        Assert.True(result.IsSuccess);
        var lesson = result.Value;
        Assert.Equal(thirdLessonId, lesson.Id);
        Assert.Equal("Moved Third", lesson.Title);
        Assert.Equal("Moved content", lesson.Content);
        Assert.Equal(1, lesson.Position);

        await _fixture.ExecuteInScopeAsync(async serviceProvider =>
        {
            var coursesDbContext = serviceProvider.GetRequiredService<CoursesDbContext>();

            var lessons = await coursesDbContext.Lessons
                .AsNoTracking()
                .Where(x => x.ModuleId == moduleId)
                .OrderBy(x => x.Position)
                .ToListAsync();

            Assert.Equal([thirdLessonId, firstLessonId, secondLessonId], lessons.Select(x => x.Id).ToArray());
            Assert.Equal([1, 2, 3], lessons.Select(x => x.Position).ToArray());
            Assert.Equal("Moved Third", lessons[0].Title);
            Assert.Equal("Moved content", lessons[0].Content);
        });
    }

    [Fact]
    public async Task UpdateLesson_WithInvalidPosition_ShouldReturnInvalidPosition_AndKeepOrder()
    {
        var authorId = await _fixture.SeedUserAsync();
        var courseId = await _fixture.SeedCourseAsync(authorId);
        var moduleId = await _fixture.SeedCourseModuleAsync(courseId);

        var firstLessonId = await _fixture.SeedLessonAsync(moduleId, "First", "First content", 1);
        var secondLessonId = await _fixture.SeedLessonAsync(moduleId, "Second", "Second content", 2);

        var result = await _fixture.ExecuteInScopeAsync(async serviceProvider =>
        {
            var handler = serviceProvider
                .GetRequiredService<ICommandHandler<UpdateLessonCommand, LessonModel>>();

            return await handler.HandleAsync(
                new UpdateLessonCommand(
                    courseId,
                    moduleId,
                    secondLessonId,
                    "Invalid Move",
                    "Invalid content",
                    3));
        });

        Assert.True(result.IsFailure);
        Assert.Equal("courses.invalid_position", result.Error.Code);
        Assert.Equal("Position must be between 1 and 2.", result.Error.Message);

        await _fixture.ExecuteInScopeAsync(async serviceProvider =>
        {
            var coursesDbContext = serviceProvider.GetRequiredService<CoursesDbContext>();

            var lessons = await coursesDbContext.Lessons
                .AsNoTracking()
                .Where(x => x.ModuleId == moduleId)
                .OrderBy(x => x.Position)
                .ToListAsync();

            Assert.Equal([firstLessonId, secondLessonId], lessons.Select(x => x.Id).ToArray());
            Assert.Equal(["First", "Second"], lessons.Select(x => x.Title).ToArray());
        });
    }

    [Fact]
    public async Task ArchiveCourseModule_ShouldMarkModuleAndLessonsArchived_AndReorderRemainingModules()
    {
        var authorId = await _fixture.SeedUserAsync();
        var courseId = await _fixture.SeedCourseAsync(authorId);

        var firstModuleId = await _fixture.SeedCourseModuleAsync(courseId, "First", "First module", 1);
        var secondModuleId = await _fixture.SeedCourseModuleAsync(courseId, "Second", "Second module", 2);
        var thirdModuleId = await _fixture.SeedCourseModuleAsync(courseId, "Third", "Third module", 3);
        var lessonId = await _fixture.SeedLessonAsync(secondModuleId, "Archived lesson", "Archived content", 1);

        var result = await _fixture.ExecuteInScopeAsync(async serviceProvider =>
        {
            var handler = serviceProvider
                .GetRequiredService<ICommandHandler<ArchiveCourseModuleCommand>>();

            return await handler.HandleAsync(new ArchiveCourseModuleCommand(courseId, secondModuleId));
        });

        Assert.True(result.IsSuccess);

        await _fixture.ExecuteInScopeAsync(async serviceProvider =>
        {
            var coursesDbContext = serviceProvider.GetRequiredService<CoursesDbContext>();

            var archivedModule = await coursesDbContext.CourseModules
                .AsNoTracking()
                .SingleAsync(x => x.Id == secondModuleId);

            var archivedLesson = await coursesDbContext.Lessons
                .AsNoTracking()
                .SingleAsync(x => x.Id == lessonId);

            var activeModules = await coursesDbContext.CourseModules
                .AsNoTracking()
                .Where(x => x.CourseId == courseId && !x.IsArchived)
                .OrderBy(x => x.Position)
                .ToListAsync();

            Assert.True(archivedModule.IsArchived);
            Assert.NotNull(archivedModule.ArchivedAt);
            Assert.True(archivedLesson.IsArchived);
            Assert.NotNull(archivedLesson.ArchivedAt);
            Assert.Equal([firstModuleId, thirdModuleId], activeModules.Select(x => x.Id).ToArray());
            Assert.Equal([1, 2], activeModules.Select(x => x.Position).ToArray());
        });
    }

    [Fact]
    public async Task ArchiveLesson_ShouldMarkLessonArchived_AndReorderRemainingLessons()
    {
        var authorId = await _fixture.SeedUserAsync();
        var courseId = await _fixture.SeedCourseAsync(authorId);
        var moduleId = await _fixture.SeedCourseModuleAsync(courseId);

        var firstLessonId = await _fixture.SeedLessonAsync(moduleId, "First", "First content", 1);
        var secondLessonId = await _fixture.SeedLessonAsync(moduleId, "Second", "Second content", 2);
        var thirdLessonId = await _fixture.SeedLessonAsync(moduleId, "Third", "Third content", 3);

        var result = await _fixture.ExecuteInScopeAsync(async serviceProvider =>
        {
            var handler = serviceProvider
                .GetRequiredService<ICommandHandler<ArchiveLessonCommand>>();

            return await handler.HandleAsync(new ArchiveLessonCommand(courseId, moduleId, secondLessonId));
        });

        Assert.True(result.IsSuccess);

        await _fixture.ExecuteInScopeAsync(async serviceProvider =>
        {
            var coursesDbContext = serviceProvider.GetRequiredService<CoursesDbContext>();

            var archivedLesson = await coursesDbContext.Lessons
                .AsNoTracking()
                .SingleAsync(x => x.Id == secondLessonId);

            var activeLessons = await coursesDbContext.Lessons
                .AsNoTracking()
                .Where(x => x.ModuleId == moduleId && !x.IsArchived)
                .OrderBy(x => x.Position)
                .ToListAsync();

            Assert.True(archivedLesson.IsArchived);
            Assert.NotNull(archivedLesson.ArchivedAt);
            Assert.Equal([firstLessonId, thirdLessonId], activeLessons.Select(x => x.Id).ToArray());
            Assert.Equal([1, 2], activeLessons.Select(x => x.Position).ToArray());
        });
    }

    [Fact]
    public async Task GetCourse_ShouldReturnExistingCourse()
    {
        var authorId = await _fixture.SeedUserAsync();
        var courseId = await _fixture.SeedCourseAsync(
            authorId,
            "Algorithms",
            "Computer Science",
            "Sorting and graphs");

        var result = await _fixture.ExecuteInScopeAsync(async serviceProvider =>
        {
            var handler = serviceProvider
                .GetRequiredService<IQueryHandler<GetCourseQuery, CourseModel>>();

            return await handler.Handle(new GetCourseQuery(courseId));
        });

        Assert.True(result.IsSuccess);
        var course = result.Value;
        Assert.Equal(courseId, course.Id);
        Assert.Equal(authorId, course.AuthorId);
        Assert.Equal("Algorithms", course.Title);
        Assert.Equal("Computer Science", course.Theme);
        Assert.Equal("Sorting and graphs", course.Description);
    }

    [Fact]
    public async Task GetCourses_ShouldReturnAllPersistedCourses()
    {
        var authorId = await _fixture.SeedUserAsync();

        var firstCourseId = await _fixture.SeedCourseAsync(
            authorId,
            "Algorithms",
            "Computer Science",
            "Sorting and graphs");

        var secondCourseId = await _fixture.SeedCourseAsync(
            authorId,
            "Databases",
            "Backend",
            "Indexes and transactions");

        var result = await _fixture.ExecuteInScopeAsync(async serviceProvider =>
        {
            var handler = serviceProvider
                .GetRequiredService<IQueryHandler<GetCoursesQuery, IReadOnlyList<CourseModel>>>();

            return await handler.Handle(new GetCoursesQuery());
        });

        Assert.True(result.IsSuccess);
        var courses = result.Value;
        Assert.Equal(2, courses.Count);
        Assert.Contains(courses, x => x.Id == firstCourseId && x.Title == "Algorithms");
        Assert.Contains(courses, x => x.Id == secondCourseId && x.Title == "Databases");
    }

    [Fact]
    public async Task UpdateCourse_ShouldUpdateCourse_AndPreserveAuthorIdAndCreatedAt()
    {
        var authorId = await _fixture.SeedUserAsync();
        var courseId = await _fixture.SeedCourseAsync(
            authorId,
            "Databases",
            "Backend",
            "Old description");

        var originalCourse = await _fixture.ExecuteInScopeAsync(async serviceProvider =>
        {
            var coursesDbContext = serviceProvider.GetRequiredService<CoursesDbContext>();

            return await coursesDbContext.Courses
                .AsNoTracking()
                .SingleAsync(x => x.Id == courseId);
        });

        var result = await _fixture.ExecuteInScopeAsync(async serviceProvider =>
        {
            var handler = serviceProvider
                .GetRequiredService<ICommandHandler<UpdateCourseCommand, CourseModel>>();

            return await handler.HandleAsync(
                new UpdateCourseCommand(
                    courseId,
                    "Databases Advanced",
                    "Backend Engineering",
                    "New description"));
        });

        Assert.True(result.IsSuccess);
        var course = result.Value;

        Assert.Equal(courseId, course.Id);
        Assert.Equal(originalCourse.AuthorId, course.AuthorId);
        Assert.Equal(originalCourse.CreatedAt, course.CreatedAt);
        Assert.Equal("Databases Advanced", course.Title);
        Assert.Equal("Backend Engineering", course.Theme);
        Assert.Equal("New description", course.Description);

        await _fixture.ExecuteInScopeAsync(async serviceProvider =>
        {
            var coursesDbContext = serviceProvider.GetRequiredService<CoursesDbContext>();

            var courseInDb = await coursesDbContext.Courses
                .AsNoTracking()
                .SingleAsync(x => x.Id == courseId);

            Assert.Equal(originalCourse.AuthorId, courseInDb.AuthorId);
            Assert.Equal(originalCourse.CreatedAt, courseInDb.CreatedAt);
            Assert.Equal("Databases Advanced", courseInDb.Title);
            Assert.Equal("Backend Engineering", courseInDb.Theme);
            Assert.Equal("New description", courseInDb.Description);
        });
    }

    [Fact]
    public async Task DeleteCourse_ShouldRemoveCourse_AndUpdateMetrics()
    {
        var authorId = await _fixture.SeedUserAsync();

        var created = await _fixture.ExecuteInScopeAsync(async serviceProvider =>
        {
            var createHandler = serviceProvider
                .GetRequiredService<ICommandHandler<CreateCourseCommand, CourseModel>>();

            return await createHandler.HandleAsync(
                new CreateCourseCommand(
                    authorId,
                    "Testing",
                    "QA",
                    "Integration tests"));
        });

        Assert.True(created.IsSuccess);
        var createdCourse = created.Value;

        var deleted = await _fixture.ExecuteInScopeAsync(async serviceProvider =>
        {
            var deleteHandler = serviceProvider
                .GetRequiredService<ICommandHandler<DeleteCourseCommand>>();

            return await deleteHandler.HandleAsync(
                new DeleteCourseCommand(createdCourse.Id));
        });

        Assert.True(deleted.IsSuccess);

        await _fixture.ExecuteInScopeAsync(async serviceProvider =>
        {
            var coursesDbContext = serviceProvider.GetRequiredService<CoursesDbContext>();
            var metrics = serviceProvider.GetRequiredService<AppMetrics>();

            var courseInDb = await coursesDbContext.Courses
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == createdCourse.Id);

            Assert.Null(courseInDb);
            Assert.Equal(0, metrics.CoursesTotal);
        });
    }
}
