using LMS.Common.CQRS;
using LMS.Common.Observability.Metrics;
using LMS.Courses.Application.Commands.CreateCourse;
using LMS.Courses.Application.Commands.DeleteCourse;
using LMS.Courses.Application.Commands.UpdateCourse;
using LMS.Courses.Application.Queries.GetCourse;
using LMS.Courses.Application.Queries.GetCourses;
using LMS.Courses.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CourseModel = LMS.Courses.Application.Models.Course;

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
