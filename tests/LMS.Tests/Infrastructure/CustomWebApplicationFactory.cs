using System.Linq;
using LMS.Courses.Infrastructure.DbContexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

namespace LMS.Tests.Infrastructure;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private SqliteConnection _connection = null!;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<CoursesDbContext>));
            services.RemoveAll(typeof(CoursesDbContext));
            services.RemoveAll(typeof(IDbContextOptionsConfiguration<CoursesDbContext>));

            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            services.AddDbContext<CoursesDbContext>(options =>
            {
                options.UseSqlite(_connection);
            });

            var sp = services.BuildServiceProvider();

            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<CoursesDbContext>();
            db.Database.EnsureCreated();
        });
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public new async Task DisposeAsync()
    {
        await base.DisposeAsync();

        if (_connection is not null)
        {
            await _connection.DisposeAsync();
        }
    }
}