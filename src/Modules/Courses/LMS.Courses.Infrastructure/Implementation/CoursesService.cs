using LMS.Courses.Core.Models;
using LMS.Courses.Core.Services;
using LMS.Courses.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace LMS.Courses.Infrastructure.Implementation;

public class CoursesService : ICoursesService
{
    private readonly CoursesDbContext _dbContext;

    public CoursesService(CoursesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CreateCourseAsync(Course course, CancellationToken cancellationToken = default)
    {
        _dbContext.Courses.Add(new()
        {
            Id =  course.Id,
            AuthorId = course.AuthorId,
            Title =  course.Title,
            Theme = course.Theme,
            Description = course.Description,
            CreatedAt =  course.CreatedAt,
            UpdatedAt = course.UpdatedAt
        });

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> UpdateCourseAsync(Course updatedCourse, CancellationToken cancellationToken = default)
    {
        var course = await _dbContext.Courses.FirstOrDefaultAsync(c => c.Id == updatedCourse.Id, cancellationToken);

        if (course is null)
        {
            return false;
        }
        
        course.Title = updatedCourse.Title;
        course.Theme = updatedCourse.Theme;
        course.Description = updatedCourse.Description;
        course.UpdatedAt = updatedCourse.UpdatedAt;

        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return true;
    }

    public async Task<bool> DeleteCourseAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        var course = await _dbContext.Courses.FirstOrDefaultAsync(c => c.Id == courseId, cancellationToken);

        if (course is null)
        {
            return false;
        }
        
        _dbContext.Courses.Remove(course);
        
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return true;
    }

    public async Task<Course?> GetCourseAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        var course =  await _dbContext.Courses.AsNoTracking().FirstOrDefaultAsync(c => c.Id == courseId, cancellationToken);

        if (course is null)
        {
            return null;
        }

        return new()
        {
            Id = course.Id,
            AuthorId = course.AuthorId,
            Title = course.Title,
            Theme = course.Theme,
            Description = course.Description,
            CreatedAt = course.CreatedAt,
            UpdatedAt = course.UpdatedAt
        };
    }

    public Task<List<Course>> GetCoursesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.Courses.AsNoTracking().Select(c => new Course()
        {
            Id = c.Id,
            AuthorId = c.AuthorId,
            Title = c.Title,
            Theme = c.Theme,
            Description = c.Description,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        }).ToListAsync(cancellationToken: cancellationToken);
    }
}