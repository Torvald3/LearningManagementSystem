using LMS.Courses.Core.Models;

namespace LMS.Courses.Core.Services;

public interface ICoursesService
{
    Task CreateCourseAsync(Course course, CancellationToken cancellationToken = default);
    
    Task<bool> UpdateCourseAsync(Course updatedCourse, CancellationToken cancellationToken = default);
    
    Task<bool> DeleteCourseAsync(Guid courseId, CancellationToken cancellationToken = default);
    
    Task<Course?> GetCourseAsync(Guid courseId, CancellationToken cancellationToken = default);
    
    Task<List<Course>> GetCoursesAsync(CancellationToken cancellationToken = default);
}