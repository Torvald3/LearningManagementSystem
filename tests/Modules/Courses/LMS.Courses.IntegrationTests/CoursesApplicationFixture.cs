using LMS.Common.Database.Configuration;
using LMS.Common.Observability.Metrics;
using LMS.Courses.Infrastructure.DbContexts;
using LMS.Users.Contracts.Models;
using LMS.Users.Contracts.Services;
using LMS.Users.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace LMS.Courses.IntegrationTests;

public sealed class CoursesApplicationFixture : IAsyncLifetime
{
    private const string DatabaseName = "lms_db";
    private const string DatabaseUsername = "postgres";
    private const string DatabasePassword = "postgres";
    private const int PostgreSqlPort = 5432;

    private readonly PostgreSqlContainer _database = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithDatabase(DatabaseName)
        .WithUsername(DatabaseUsername)
        .WithPassword(DatabasePassword)
        .Build();

    public IServiceProvider Services { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await _database.StartAsync();

        var services = new ServiceCollection();

        services.AddLogging();
        services.AddSingleton<AppMetrics>();

        var databaseConfiguration = new DatabaseConfiguration
        {
            Server = _database.Hostname,
            Port = _database.GetMappedPublicPort(PostgreSqlPort),
            Name = DatabaseName,
            Username = DatabaseUsername,
            Password = DatabasePassword
        };

        LMS.Users.Infrastructure.Extensions.ServiceCollectionExtensions.AddInfrastructureServices(
            services,
            databaseConfiguration);

        LMS.Users.Application.Extensions.ServiceCollectionExtensions.AddApplicationServices(
            services);

        LMS.Courses.Infrastructure.Extensions.ServiceCollectionExtensions.AddInfrastructureServices(
            services,
            databaseConfiguration);

        LMS.Courses.Application.Extensions.ServiceCollectionExtensions.AddApplicationServices(
            services);

        Services = services.BuildServiceProvider(validateScopes: true);

        await CreateDatabaseObjectsAsync();
        await ResetDatabaseAsync();
    }

    public async Task DisposeAsync()
    {
        if (Services is IAsyncDisposable asyncDisposable)
        {
            await asyncDisposable.DisposeAsync();
        }
        else if (Services is IDisposable disposable)
        {
            disposable.Dispose();
        }

        await _database.DisposeAsync();
    }

    public async Task ResetDatabaseAsync()
    {
        await ExecuteInScopeAsync(async serviceProvider =>
        {
            var coursesDbContext = serviceProvider.GetRequiredService<CoursesDbContext>();
            var usersDbContext = serviceProvider.GetRequiredService<UsersDbContext>();
            var metrics = serviceProvider.GetRequiredService<AppMetrics>();

            await coursesDbContext.Database.ExecuteSqlRawAsync("""
                TRUNCATE TABLE courses.course RESTART IDENTITY CASCADE;
                """);

            await usersDbContext.Database.ExecuteSqlRawAsync("""
                TRUNCATE TABLE users.users, users.contacts RESTART IDENTITY CASCADE;
                """);

            metrics.Reset();
        });
    }

    public async Task<Guid> SeedUserAsync(string? email = null, string? username = null)
    {
        var userId = Guid.NewGuid();

        await ExecuteInScopeAsync(async serviceProvider =>
        {
            var usersModuleService = serviceProvider.GetRequiredService<IUsersModuleService>();

            await usersModuleService.CreateUserAsync(
                new CreateUserRequest(
                    userId,
                    email ?? $"{userId:N}@tests.local",
                    username ?? $"user_{Guid.NewGuid():N}"[..16]));
        });

        return userId;
    }

    public async Task<Guid> SeedCourseAsync(
        Guid authorId,
        string? title = null,
        string? theme = null,
        string? description = null)
    {
        var courseId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        await ExecuteInScopeAsync(async serviceProvider =>
        {
            var coursesDbContext = serviceProvider.GetRequiredService<CoursesDbContext>();

            coursesDbContext.Courses.Add(new LMS.Courses.Infrastructure.Entities.Course
            {
                Id = courseId,
                AuthorId = authorId,
                Title = title ?? "Algorithms",
                Theme = theme ?? "Computer Science",
                Description = description ?? "Sorting and graphs",
                CreatedAt = now,
                UpdatedAt = now
            });

            await coursesDbContext.SaveChangesAsync();
        });

        return courseId;
    }

    public async Task<TResult> ExecuteInScopeAsync<TResult>(
        Func<IServiceProvider, Task<TResult>> action)
    {
        await using var scope = Services.CreateAsyncScope();
        return await action(scope.ServiceProvider);
    }

    public async Task ExecuteInScopeAsync(
        Func<IServiceProvider, Task> action)
    {
        await using var scope = Services.CreateAsyncScope();
        await action(scope.ServiceProvider);
    }

    private async Task CreateDatabaseObjectsAsync()
    {
        await ExecuteInScopeAsync(async serviceProvider =>
        {
            var usersDbContext = serviceProvider.GetRequiredService<UsersDbContext>();
            var coursesDbContext = serviceProvider.GetRequiredService<CoursesDbContext>();

            await CreateSchemaFromModelAsync(usersDbContext);
            await CreateSchemaFromModelAsync(coursesDbContext);
        });
    }

    private static async Task CreateSchemaFromModelAsync(DbContext dbContext)
    {
        var createScript = dbContext.Database.GenerateCreateScript();

        if (!string.IsNullOrWhiteSpace(createScript))
        {
            await dbContext.Database.ExecuteSqlRawAsync(createScript);
        }
    }
}
