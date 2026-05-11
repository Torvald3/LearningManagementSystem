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
                .GetRequiredService<ICommandHandler<CreateCourseCommand, CreateCourseResult>>();

            return await handler.HandleAsync(
                new CreateCourseCommand(
                    authorId,
                    "C# Basics",
                    "Programming",
                    "Intro course"));
        });

        Assert.Equal(CreateCourseStatus.Success, result.Status);
        Assert.NotNull(result.Course);
        Assert.Equal(authorId, result.Course!.AuthorId);
        Assert.Equal("C# Basics", result.Course.Title);
        Assert.Equal("Programming", result.Course.Theme);
        Assert.Equal("Intro course", result.Course.Description);

        await _fixture.ExecuteInScopeAsync(async serviceProvider =>
        {
            var coursesDbContext = serviceProvider.GetRequiredService<CoursesDbContext>();
            var metrics = serviceProvider.GetRequiredService<AppMetrics>();

            var courseInDb = await coursesDbContext.Courses
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == result.Course.Id);

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
                .GetRequiredService<ICommandHandler<CreateCourseCommand, CreateCourseResult>>();

            return await handler.HandleAsync(
                new CreateCourseCommand(
                    Guid.NewGuid(),
                    "C# Basics",
                    "Programming",
                    "Intro course"));
        });

        Assert.Equal(CreateCourseStatus.AuthorNotFound, result.Status);
        Assert.Null(result.Course);
        Assert.Contains("Author does not exist.", result.Errors);

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
                .GetRequiredService<ICommandHandler<CreateCourseModuleCommand, CreateCourseModuleResult>>();

            return await handler.HandleAsync(
                new CreateCourseModuleCommand(
                    courseId,
                    "Getting Started",
                    "Course introduction"));
        });

        Assert.Equal(CreateCourseModuleStatus.Success, result.Status);
        Assert.NotNull(result.Module);
        Assert.Equal(courseId, result.Module!.CourseId);
        Assert.Equal("Getting Started", result.Module.Title);
        Assert.Equal("Course introduction", result.Module.Description);
        Assert.Equal(1, result.Module.Position);

        await _fixture.ExecuteInScopeAsync(async serviceProvider =>
        {
            var coursesDbContext = serviceProvider.GetRequiredService<CoursesDbContext>();

            var moduleInDb = await coursesDbContext.CourseModules
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == result.Module.Id);

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
                .GetRequiredService<ICommandHandler<CreateCourseModuleCommand, CreateCourseModuleResult>>();

            return await handler.HandleAsync(
                new CreateCourseModuleCommand(
                    Guid.NewGuid(),
                    "Getting Started",
                    "Course introduction"));
        });

        Assert.Equal(CreateCourseModuleStatus.CourseNotFound, result.Status);
        Assert.Null(result.Module);
        Assert.Contains("Course not found.", result.Errors);

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
                .GetRequiredService<ICommandHandler<CreateLessonCommand, CreateLessonResult>>();

            return await handler.HandleAsync(
                new CreateLessonCommand(
                    courseId,
                    moduleId,
                    "Variables",
                    "Working with variables"));
        });

        Assert.Equal(CreateLessonStatus.Success, result.Status);
        Assert.NotNull(result.Lesson);
        Assert.Equal(moduleId, result.Lesson!.ModuleId);
        Assert.Equal("Variables", result.Lesson.Title);
        Assert.Equal("Working with variables", result.Lesson.Content);
        Assert.Equal(1, result.Lesson.Position);

        await _fixture.ExecuteInScopeAsync(async serviceProvider =>
        {
            var coursesDbContext = serviceProvider.GetRequiredService<CoursesDbContext>();

            var lessonInDb = await coursesDbContext.Lessons
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == result.Lesson.Id);

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
                .GetRequiredService<ICommandHandler<CreateLessonCommand, CreateLessonResult>>();

            return await handler.HandleAsync(
                new CreateLessonCommand(
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    "Variables",
                    "Working with variables"));
        });

        Assert.Equal(CreateLessonStatus.CourseNotFound, result.Status);
        Assert.Null(result.Lesson);
        Assert.Contains("Course not found.", result.Errors);

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
                .GetRequiredService<ICommandHandler<CreateLessonCommand, CreateLessonResult>>();

            return await handler.HandleAsync(
                new CreateLessonCommand(
                    courseId,
                    Guid.NewGuid(),
                    "Variables",
                    "Working with variables"));
        });

        Assert.Equal(CreateLessonStatus.ModuleNotFound, result.Status);
        Assert.Null(result.Lesson);
        Assert.Contains("Module not found.", result.Errors);

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
                .GetRequiredService<IQueryHandler<GetCourseModuleQuery, CourseModuleModel?>>();

            return await handler.Handle(new GetCourseModuleQuery(courseId, moduleId));
        });

        Assert.NotNull(result);
        Assert.Equal(moduleId, result!.Id);
        Assert.Equal(courseId, result.CourseId);
        Assert.Equal("Basics", result.Title);
        Assert.Equal("Start here", result.Description);
        Assert.Equal(3, result.Position);
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
                .GetRequiredService<IQueryHandler<GetCourseModulesQuery, IReadOnlyList<CourseModuleSummaryModel>?>>();

            return await handler.Handle(new GetCourseModulesQuery(courseId));
        });

        Assert.NotNull(result);
        Assert.Equal(2, result!.Count);
        Assert.Equal(firstModuleId, result[0].Id);
        Assert.Equal(2, result[0].LessonsCount);
        Assert.Equal(secondModuleId, result[1].Id);
        Assert.Equal(1, result[1].LessonsCount);
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
                .GetRequiredService<IQueryHandler<GetLessonQuery, LessonModel?>>();

            return await handler.Handle(new GetLessonQuery(courseId, moduleId, lessonId));
        });

        Assert.NotNull(result);
        Assert.Equal(lessonId, result!.Id);
        Assert.Equal(moduleId, result.ModuleId);
        Assert.Equal("Variables", result.Title);
        Assert.Equal("Full lesson content", result.Content);
        Assert.Equal(5, result.Position);
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
                .GetRequiredService<IQueryHandler<GetLessonsQuery, IReadOnlyList<LessonSummaryModel>?>>();

            return await handler.Handle(new GetLessonsQuery(courseId, moduleId));
        });

        Assert.NotNull(result);
        Assert.Equal(2, result!.Count);
        Assert.Equal(firstLessonId, result[0].Id);
        Assert.Equal("First lesson", result[0].Title);
        Assert.Equal(1, result[0].Position);
        Assert.Equal(secondLessonId, result[1].Id);
        Assert.Equal("Second lesson", result[1].Title);
        Assert.Equal(2, result[1].Position);
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
                .GetRequiredService<ICommandHandler<UpdateCourseModuleCommand, UpdateCourseModuleResult>>();

            return await handler.HandleAsync(
                new UpdateCourseModuleCommand(
                    courseId,
                    thirdModuleId,
                    "Moved Third",
                    "Moved module",
                    1));
        });

        Assert.Equal(UpdateCourseModuleStatus.Success, result.Status);
        Assert.NotNull(result.Module);
        Assert.Equal(thirdModuleId, result.Module!.Id);
        Assert.Equal("Moved Third", result.Module.Title);
        Assert.Equal("Moved module", result.Module.Description);
        Assert.Equal(1, result.Module.Position);

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
                .GetRequiredService<ICommandHandler<UpdateCourseModuleCommand, UpdateCourseModuleResult>>();

            return await handler.HandleAsync(
                new UpdateCourseModuleCommand(
                    courseId,
                    secondModuleId,
                    "Invalid Move",
                    "Invalid module",
                    3));
        });

        Assert.Equal(UpdateCourseModuleStatus.InvalidPosition, result.Status);
        Assert.Null(result.Module);
        Assert.Contains("Position must be between 1 and 2.", result.Errors);

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
                .GetRequiredService<ICommandHandler<UpdateLessonCommand, UpdateLessonResult>>();

            return await handler.HandleAsync(
                new UpdateLessonCommand(
                    courseId,
                    moduleId,
                    thirdLessonId,
                    "Moved Third",
                    "Moved content",
                    1));
        });

        Assert.Equal(UpdateLessonStatus.Success, result.Status);
        Assert.NotNull(result.Lesson);
        Assert.Equal(thirdLessonId, result.Lesson!.Id);
        Assert.Equal("Moved Third", result.Lesson.Title);
        Assert.Equal("Moved content", result.Lesson.Content);
        Assert.Equal(1, result.Lesson.Position);

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
                .GetRequiredService<ICommandHandler<UpdateLessonCommand, UpdateLessonResult>>();

            return await handler.HandleAsync(
                new UpdateLessonCommand(
                    courseId,
                    moduleId,
                    secondLessonId,
                    "Invalid Move",
                    "Invalid content",
                    3));
        });

        Assert.Equal(UpdateLessonStatus.InvalidPosition, result.Status);
        Assert.Null(result.Lesson);
        Assert.Contains("Position must be between 1 and 2.", result.Errors);

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
                .GetRequiredService<ICommandHandler<ArchiveCourseModuleCommand, ArchiveCourseModuleResult>>();

            return await handler.HandleAsync(new ArchiveCourseModuleCommand(courseId, secondModuleId));
        });

        Assert.Equal(ArchiveCourseModuleStatus.Success, result.Status);

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
                .GetRequiredService<ICommandHandler<ArchiveLessonCommand, ArchiveLessonResult>>();

            return await handler.HandleAsync(new ArchiveLessonCommand(courseId, moduleId, secondLessonId));
        });

        Assert.Equal(ArchiveLessonStatus.Success, result.Status);

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
                .GetRequiredService<IQueryHandler<GetCourseQuery, CourseModel?>>();

            return await handler.Handle(new GetCourseQuery(courseId));
        });

        Assert.NotNull(result);
        Assert.Equal(courseId, result!.Id);
        Assert.Equal(authorId, result.AuthorId);
        Assert.Equal("Algorithms", result.Title);
        Assert.Equal("Computer Science", result.Theme);
        Assert.Equal("Sorting and graphs", result.Description);
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

        Assert.Equal(2, result.Count);
        Assert.Contains(result, x => x.Id == firstCourseId && x.Title == "Algorithms");
        Assert.Contains(result, x => x.Id == secondCourseId && x.Title == "Databases");
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
                .GetRequiredService<ICommandHandler<UpdateCourseCommand, UpdateCourseResult>>();

            return await handler.HandleAsync(
                new UpdateCourseCommand(
                    courseId,
                    "Databases Advanced",
                    "Backend Engineering",
                    "New description"));
        });

        Assert.Equal(UpdateCourseStatus.Success, result.Status);
        Assert.NotNull(result.Course);

        Assert.Equal(courseId, result.Course!.Id);
        Assert.Equal(originalCourse.AuthorId, result.Course.AuthorId);
        Assert.Equal(originalCourse.CreatedAt, result.Course.CreatedAt);
        Assert.Equal("Databases Advanced", result.Course.Title);
        Assert.Equal("Backend Engineering", result.Course.Theme);
        Assert.Equal("New description", result.Course.Description);

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
                .GetRequiredService<ICommandHandler<CreateCourseCommand, CreateCourseResult>>();

            return await createHandler.HandleAsync(
                new CreateCourseCommand(
                    authorId,
                    "Testing",
                    "QA",
                    "Integration tests"));
        });

        Assert.NotNull(created.Course);

        var deleted = await _fixture.ExecuteInScopeAsync(async serviceProvider =>
        {
            var deleteHandler = serviceProvider
                .GetRequiredService<ICommandHandler<DeleteCourseCommand, bool>>();

            return await deleteHandler.HandleAsync(
                new DeleteCourseCommand(created.Course!.Id));
        });

        Assert.True(deleted);

        await _fixture.ExecuteInScopeAsync(async serviceProvider =>
        {
            var coursesDbContext = serviceProvider.GetRequiredService<CoursesDbContext>();
            var metrics = serviceProvider.GetRequiredService<AppMetrics>();

            var courseInDb = await coursesDbContext.Courses
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == created.Course!.Id);

            Assert.Null(courseInDb);
            Assert.Equal(0, metrics.CoursesTotal);
        });
    }
}
