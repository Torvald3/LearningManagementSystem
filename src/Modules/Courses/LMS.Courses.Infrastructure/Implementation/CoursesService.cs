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

        _dbContext.CourseMembers.Add(new Entities.CourseMember
        {
            Id = Guid.NewGuid(),
            CourseId = course.Id,
            UserId = course.AuthorId,
            Role = CourseRole.CourseOwner,
            CreatedAt = course.CreatedAt,
            UpdatedAt = course.UpdatedAt
        });

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task CreateCourseMemberAsync(CourseMember member, CancellationToken cancellationToken = default)
    {
        _dbContext.CourseMembers.Add(new Entities.CourseMember
        {
            Id = member.Id,
            CourseId = member.CourseId,
            UserId = member.UserId,
            Role = member.Role,
            CreatedAt = member.CreatedAt,
            UpdatedAt = member.UpdatedAt
        });

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<CourseMember?> GetCourseMemberAsync(
        Guid courseId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var member = await _dbContext.CourseMembers
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.CourseId == courseId && x.UserId == userId,
                cancellationToken);

        if (member is null)
        {
            return null;
        }

        return new CourseMember
        {
            Id = member.Id,
            CourseId = member.CourseId,
            UserId = member.UserId,
            Role = member.Role,
            CreatedAt = member.CreatedAt,
            UpdatedAt = member.UpdatedAt
        };
    }

    public Task<List<CourseMember>> GetCourseMembersAsync(
        Guid courseId,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.CourseMembers
            .AsNoTracking()
            .Where(x => x.CourseId == courseId)
            .OrderBy(x => x.Role)
            .ThenBy(x => x.CreatedAt)
            .Select(x => new CourseMember
            {
                Id = x.Id,
                CourseId = x.CourseId,
                UserId = x.UserId,
                Role = x.Role,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            })
            .ToListAsync(cancellationToken);
    }

    public Task<bool> CourseOwnerExistsAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        return _dbContext.CourseMembers
            .AsNoTracking()
            .AnyAsync(
                x => x.CourseId == courseId && x.Role == CourseRole.CourseOwner,
                cancellationToken);
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

    public async Task CreateCourseModuleAsync(CourseModule module, CancellationToken cancellationToken = default)
    {
        _dbContext.CourseModules.Add(new()
        {
            Id = module.Id,
            CourseId = module.CourseId,
            Title = module.Title,
            Description = module.Description,
            Position = module.Position,
            CreatedAt = module.CreatedAt,
            UpdatedAt = module.UpdatedAt,
            IsArchived = false
        });

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<CourseModule?> GetCourseModuleAsync(
        Guid courseId,
        Guid moduleId,
        CancellationToken cancellationToken = default)
    {
        var module = await _dbContext.CourseModules
            .AsNoTracking()
            .FirstOrDefaultAsync(
                m => m.Id == moduleId && m.CourseId == courseId && !m.IsArchived,
                cancellationToken);

        if (module is null)
        {
            return null;
        }

        return new CourseModule
        {
            Id = module.Id,
            CourseId = module.CourseId,
            Title = module.Title,
            Description = module.Description,
            Position = module.Position,
            CreatedAt = module.CreatedAt,
            UpdatedAt = module.UpdatedAt,
            IsArchived = module.IsArchived,
            ArchivedAt = module.ArchivedAt
        };
    }

    public Task<List<CourseModuleSummary>> GetCourseModulesAsync(
        Guid courseId,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.CourseModules
            .AsNoTracking()
            .Where(m => m.CourseId == courseId && !m.IsArchived)
            .OrderBy(m => m.Position)
            .Select(m => new CourseModuleSummary
            {
                Id = m.Id,
                CourseId = m.CourseId,
                Title = m.Title,
                Description = m.Description,
                Position = m.Position,
                LessonsCount = m.Lessons.Count(l => !l.IsArchived)
            })
            .ToListAsync(cancellationToken);
    }

    public Task<int> GetCourseModulesCountAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        return _dbContext.CourseModules
            .AsNoTracking()
            .CountAsync(m => m.CourseId == courseId && !m.IsArchived, cancellationToken);
    }

    public async Task<int> GetNextCourseModulePositionAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        var lastPosition = await _dbContext.CourseModules
            .AsNoTracking()
            .Where(m => m.CourseId == courseId && !m.IsArchived)
            .Select(m => (int?)m.Position)
            .MaxAsync(cancellationToken);

        return (lastPosition ?? 0) + 1;
    }

    public async Task<bool> UpdateCourseModuleAsync(
        CourseModule updatedModule,
        CancellationToken cancellationToken = default)
    {
        var modules = await _dbContext.CourseModules
            .Where(m => m.CourseId == updatedModule.CourseId && !m.IsArchived)
            .OrderBy(m => m.Position)
            .ThenBy(m => m.CreatedAt)
            .ToListAsync(cancellationToken);

        var module = modules.SingleOrDefault(m => m.Id == updatedModule.Id);

        if (module is null || updatedModule.Position < 1 || updatedModule.Position > modules.Count)
        {
            return false;
        }

        module.Title = updatedModule.Title;
        module.Description = updatedModule.Description;
        module.UpdatedAt = updatedModule.UpdatedAt;

        modules.Remove(module);
        modules.Insert(updatedModule.Position - 1, module);

        for (var i = 0; i < modules.Count; i++)
        {
            modules[i].Position = i + 1;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> ArchiveCourseModuleAsync(
        Guid courseId,
        Guid moduleId,
        DateTime archivedAt,
        CancellationToken cancellationToken = default)
    {
        var modules = await _dbContext.CourseModules
            .Where(m => m.CourseId == courseId && !m.IsArchived)
            .OrderBy(m => m.Position)
            .ThenBy(m => m.CreatedAt)
            .ToListAsync(cancellationToken);

        var module = modules.SingleOrDefault(m => m.Id == moduleId);

        if (module is null)
        {
            return false;
        }

        module.IsArchived = true;
        module.ArchivedAt = archivedAt;
        module.UpdatedAt = archivedAt;

        var lessons = await _dbContext.Lessons
            .Where(l => l.ModuleId == moduleId && !l.IsArchived)
            .ToListAsync(cancellationToken);

        foreach (var lesson in lessons)
        {
            lesson.IsArchived = true;
            lesson.ArchivedAt = archivedAt;
            lesson.UpdatedAt = archivedAt;
        }

        modules.Remove(module);

        for (var i = 0; i < modules.Count; i++)
        {
            modules[i].Position = i + 1;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task CreateLessonAsync(Lesson lesson, CancellationToken cancellationToken = default)
    {
        _dbContext.Lessons.Add(new()
        {
            Id = lesson.Id,
            ModuleId = lesson.ModuleId,
            Title = lesson.Title,
            Content = lesson.Content,
            Position = lesson.Position,
            CreatedAt = lesson.CreatedAt,
            UpdatedAt = lesson.UpdatedAt,
            IsArchived = false
        });

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Lesson?> GetLessonAsync(
        Guid moduleId,
        Guid lessonId,
        CancellationToken cancellationToken = default)
    {
        var lesson = await _dbContext.Lessons
            .AsNoTracking()
            .FirstOrDefaultAsync(
                l => l.Id == lessonId && l.ModuleId == moduleId && !l.IsArchived,
                cancellationToken);

        if (lesson is null)
        {
            return null;
        }

        return new Lesson
        {
            Id = lesson.Id,
            ModuleId = lesson.ModuleId,
            Title = lesson.Title,
            Content = lesson.Content,
            Position = lesson.Position,
            CreatedAt = lesson.CreatedAt,
            UpdatedAt = lesson.UpdatedAt,
            IsArchived = lesson.IsArchived,
            ArchivedAt = lesson.ArchivedAt
        };
    }

    public Task<List<LessonSummary>> GetLessonsAsync(Guid moduleId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Lessons
            .AsNoTracking()
            .Where(l => l.ModuleId == moduleId && !l.IsArchived)
            .OrderBy(l => l.Position)
            .Select(l => new LessonSummary
            {
                Id = l.Id,
                ModuleId = l.ModuleId,
                Title = l.Title,
                Position = l.Position
            })
            .ToListAsync(cancellationToken);
    }

    public Task<int> GetLessonsCountAsync(Guid moduleId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Lessons
            .AsNoTracking()
            .CountAsync(l => l.ModuleId == moduleId && !l.IsArchived, cancellationToken);
    }

    public async Task<int> GetNextLessonPositionAsync(Guid moduleId, CancellationToken cancellationToken = default)
    {
        var lastPosition = await _dbContext.Lessons
            .AsNoTracking()
            .Where(l => l.ModuleId == moduleId && !l.IsArchived)
            .Select(l => (int?)l.Position)
            .MaxAsync(cancellationToken);

        return (lastPosition ?? 0) + 1;
    }

    public async Task<bool> UpdateLessonAsync(Lesson updatedLesson, CancellationToken cancellationToken = default)
    {
        var lessons = await _dbContext.Lessons
            .Where(l => l.ModuleId == updatedLesson.ModuleId && !l.IsArchived)
            .OrderBy(l => l.Position)
            .ThenBy(l => l.CreatedAt)
            .ToListAsync(cancellationToken);

        var lesson = lessons.SingleOrDefault(l => l.Id == updatedLesson.Id);

        if (lesson is null || updatedLesson.Position < 1 || updatedLesson.Position > lessons.Count)
        {
            return false;
        }

        lesson.Title = updatedLesson.Title;
        lesson.Content = updatedLesson.Content;
        lesson.UpdatedAt = updatedLesson.UpdatedAt;

        lessons.Remove(lesson);
        lessons.Insert(updatedLesson.Position - 1, lesson);

        for (var i = 0; i < lessons.Count; i++)
        {
            lessons[i].Position = i + 1;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> ArchiveLessonAsync(
        Guid moduleId,
        Guid lessonId,
        DateTime archivedAt,
        CancellationToken cancellationToken = default)
    {
        var lessons = await _dbContext.Lessons
            .Where(l => l.ModuleId == moduleId && !l.IsArchived)
            .OrderBy(l => l.Position)
            .ThenBy(l => l.CreatedAt)
            .ToListAsync(cancellationToken);

        var lesson = lessons.SingleOrDefault(l => l.Id == lessonId);

        if (lesson is null)
        {
            return false;
        }

        lesson.IsArchived = true;
        lesson.ArchivedAt = archivedAt;
        lesson.UpdatedAt = archivedAt;

        lessons.Remove(lesson);

        for (var i = 0; i < lessons.Count; i++)
        {
            lessons[i].Position = i + 1;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}
