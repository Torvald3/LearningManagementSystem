using System.Net;
using System.Net.Http.Json;
using LMS.Common.Observability.Metrics;
using LMS.Courses.Api.Models;
using LMS.Courses.Core.Models;
using LMS.Courses.Infrastructure.DbContexts;
using LMS.Tests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace LMS.Tests.Courses;

public class CourseEndpointsIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public CourseEndpointsIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }
    private async Task ResetStateAsync()
    {
        using var scope = _factory.Services.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<CoursesDbContext>();
        db.Courses.RemoveRange(db.Courses);
        await db.SaveChangesAsync();

        var metrics = scope.ServiceProvider.GetRequiredService<AppMetrics>();

        metrics.Reset();
    }

    [Fact]
    public async Task Post_CreateCourse_ShouldPersistCourseInDatabase_AndReturnCreated()
    {
        // Arrange
        var request = new CreateCourseRequest
        {
            Title = "C# Basics",
            Theme = "Programming",
            Description = "Intro course"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/courses/", request);

        // Assert HTTP
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var body = await response.ReadAsJsonAsync<CourseResponse>();
        Assert.Equal("C# Basics", body.Title);
        Assert.Equal("Programming", body.Theme);
        Assert.Equal("Intro course", body.Description);
        Assert.NotEqual(Guid.Empty, body.Id);

        // Assert DB
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CoursesDbContext>();

        var courseInDb = await db.Courses.FindAsync(body.Id);
        Assert.NotNull(courseInDb);
        Assert.Equal("C# Basics", courseInDb!.Title);
        Assert.Equal("Programming", courseInDb.Theme);
        Assert.Equal("Intro course", courseInDb.Description);
    }

    [Fact]
    public async Task Post_CreateCourse_WithEmptyTitle_ShouldReturnBadRequest_AndNotPersist()
    {
        await ResetStateAsync();
        // Arrange
        var request = new CreateCourseRequest
        {
            Title = "   ",
            Theme = "Programming",
            Description = "Intro course"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/courses/", request);

        // Assert HTTP
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var error = await response.Content.ReadAsStringAsync();
        Assert.Contains("Title is required.", error);

        // Assert DB
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CoursesDbContext>();

        Assert.Empty(db.Courses);
    }

    [Fact]
    public async Task Get_CourseById_ShouldReturnExistingCourse()
    {
        // Arrange
        Guid courseId;

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<CoursesDbContext>();

            var course = new Course
            {
                Id = Guid.NewGuid(),
                Title = "Algorithms",
                Theme = "Computer Science",
                Description = "Sorting and graphs",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            db.Courses.Add(course);
            await db.SaveChangesAsync();

            courseId = course.Id;
        }

        // Act
        var response = await _client.GetAsync($"/api/courses/{courseId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.ReadAsJsonAsync<CourseResponse>();
        Assert.Equal(courseId, body.Id);
        Assert.Equal("Algorithms", body.Title);
        Assert.Equal("Computer Science", body.Theme);
        Assert.Equal("Sorting and graphs", body.Description);
    }

    [Fact]
    public async Task Put_UpdateDescription_ShouldUpdateCourseInDatabase_AndReturnUpdatedCourse()
    {
        await ResetStateAsync();
        // Arrange
        Guid courseId;

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<CoursesDbContext>();

            var course = new Course
            {
                Id = Guid.NewGuid(),
                Title = "Databases",
                Theme = "Backend",
                Description = "Old description",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            db.Courses.Add(course);
            await db.SaveChangesAsync();

            courseId = course.Id;
        }

        var request = new UpdateCourseDescriptionRequest
        {
            Description = "New description"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/courses/{courseId}/description", request);

        // Assert HTTP
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.ReadAsJsonAsync<CourseResponse>();
        Assert.Equal("New description", body.Description);

        // Assert DB
        using var verifyScope = _factory.Services.CreateScope();
        var verifyDb = verifyScope.ServiceProvider.GetRequiredService<CoursesDbContext>();

        var courseInDb = await verifyDb.Courses.FindAsync(courseId);
        Assert.NotNull(courseInDb);
        Assert.Equal("New description", courseInDb!.Description);
    }

    [Fact]
    public async Task Delete_Course_ShouldRemoveCourseFromDatabase_AndUpdateMetrics()
    {
        await ResetStateAsync();
        // Arrange
        Guid courseId;

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<CoursesDbContext>();
            var metrics = scope.ServiceProvider.GetRequiredService<AppMetrics>();

            var course = new Course
            {
                Id = Guid.NewGuid(),
                Title = "Testing",
                Theme = "QA",
                Description = "Integration tests",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            db.Courses.Add(course);
            await db.SaveChangesAsync();

            metrics.CourseCreated(course.Id);
            courseId = course.Id;
        }

        // Act
        var response = await _client.DeleteAsync($"/api/courses/{courseId}");

        // Assert HTTP
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Assert DB
        using var verifyScope = _factory.Services.CreateScope();
        var verifyDb = verifyScope.ServiceProvider.GetRequiredService<CoursesDbContext>();
        var metricsAfter = verifyScope.ServiceProvider.GetRequiredService<AppMetrics>();

        var courseInDb = await verifyDb.Courses.FindAsync(courseId);
        Assert.Null(courseInDb);
        Assert.Equal(0, metricsAfter.CoursesTotal);
    }
}