using LMS.Common.Database.Configuration;
using LMS.Courses.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace LMS.Courses.Infrastructure.DesignTime;

public class CoursesDbContextDesignTimeFactory : DesignTimeDbContextFactoryBase<CoursesDbContext>
{
    public CoursesDbContextDesignTimeFactory()
        : base("LMS.Courses.Infrastructure")
    {
    }

    protected override CoursesDbContext CreateNewInstance(DbContextOptions<CoursesDbContext> options)
    {
        return new CoursesDbContext(options);
    }

    protected override string GetConnectionString()
    {
        return DatabaseConfiguration.ConnectionString;
    }
}