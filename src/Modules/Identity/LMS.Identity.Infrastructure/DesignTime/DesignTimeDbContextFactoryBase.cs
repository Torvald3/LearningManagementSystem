using LMS.Common.Database.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace LMS.Identity.Infrastructure.DesignTime;

public abstract class DesignTimeDbContextFactoryBase<TContext> : IDesignTimeDbContextFactory<TContext>
     where TContext : DbContext
{
    protected DesignTimeDbContextFactoryBase(string migrationsAssemblyName)
    {
        MigrationsAssemblyName = migrationsAssemblyName;

        InitializeConfiguration();
    }

    protected string MigrationsAssemblyName { get; }

    protected DatabaseConfiguration DatabaseConfiguration { get; set; }

    public TContext CreateDbContext(string[] args)
    {
        return CreateContext(args);
    }

    protected virtual TContext CreateContext(string[] args)
    {
        string connectionString = GetConnectionString();
        if (!string.IsNullOrWhiteSpace(connectionString))
        {
            var optionsBuilder = new DbContextOptionsBuilder<TContext>();

            optionsBuilder.UseNpgsql(connectionString,
                                     sqlServerOptions =>
                                     {
                                         sqlServerOptions.MigrationsAssembly(MigrationsAssemblyName);   
                                         
                                         sqlServerOptions.MigrationsHistoryTable("__EFMigrationsHistory", "identity");
                                     });

            return CreateNewInstance(optionsBuilder.Options);
        }
        else
        {
            throw new InvalidOperationException($"Could not find a connection string with value '{connectionString}'.");
        }
    }

    protected abstract TContext CreateNewInstance(DbContextOptions<TContext> options);

    protected abstract string GetConnectionString();

    private void InitializeConfiguration()
    {
        string location = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\LMS.App");

        string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        VerifyAppsettingsFileAvailability(location, environment);

        var builder = new ConfigurationBuilder()
            .SetBasePath(location)
            .AddJsonFile("appsettings.json", false, true)
            .AddJsonFile($"appsettings.{environment}.json", true, true)
            .AddEnvironmentVariables();

        var configuration = builder.Build();

        DatabaseConfiguration = new DatabaseConfiguration();
        configuration.Bind("Database", DatabaseConfiguration);
    }

    private void VerifyAppsettingsFileAvailability(string location, string environment)
    {
        string appSettingsFile = Path.Combine(location, "appsettings.json");
        if (!File.Exists(appSettingsFile))
        {
            throw new FileNotFoundException($"'appsettings.json' file can't be found in '{location}' folder");
        }

        if (!string.IsNullOrEmpty(environment))
        {
            string appSettingsSpecificFile = Path.Combine(location, $"appsettings.{environment}.json");
            if (!File.Exists(appSettingsSpecificFile))
            {
                throw new FileNotFoundException($"'appsettings.{environment}.json' file can't be found in '{location}' folder");
            }
        }
    }
}
