using LMS.Courses.Core.Models;

namespace LMS.Courses.Core.Services;

public interface ICoursesService
{
    Task CreateCourseAsync(Course course, Guid ownerUserId, CancellationToken cancellationToken = default);

    Task CreateCourseMemberAsync(CourseMember member, CancellationToken cancellationToken = default);

    Task<CourseMember?> GetCourseMemberAsync(Guid courseId, Guid userId, CancellationToken cancellationToken = default);

    Task<List<CourseMember>> GetCourseMembersAsync(Guid courseId, CancellationToken cancellationToken = default);

    Task<bool> CourseOwnerExistsAsync(Guid courseId, CancellationToken cancellationToken = default);
    
    Task<bool> UpdateCourseAsync(Course updatedCourse, CancellationToken cancellationToken = default);
    
    Task<bool> DeleteCourseAsync(Guid courseId, CancellationToken cancellationToken = default);
    
    Task<Course?> GetCourseAsync(Guid courseId, CancellationToken cancellationToken = default);
    
    Task<List<Course>> GetCoursesAsync(CancellationToken cancellationToken = default);

    Task<List<Course>> GetCoursesByMemberAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<List<Course>> GetCoursesByMemberRolesAsync(
        Guid userId,
        IReadOnlyCollection<CourseRole> roles,
        CancellationToken cancellationToken = default);

    Task CreateCourseModuleAsync(CourseModule module, CancellationToken cancellationToken = default);

    Task<CourseModule?> GetCourseModuleAsync(Guid courseId, Guid moduleId, CancellationToken cancellationToken = default);

    Task<List<CourseModuleSummary>> GetCourseModulesAsync(Guid courseId, CancellationToken cancellationToken = default);

    Task<int> GetCourseModulesCountAsync(Guid courseId, CancellationToken cancellationToken = default);

    Task<int> GetNextCourseModulePositionAsync(Guid courseId, CancellationToken cancellationToken = default);

    Task<bool> UpdateCourseModuleAsync(CourseModule updatedModule, CancellationToken cancellationToken = default);

    Task<bool> ArchiveCourseModuleAsync(Guid courseId, Guid moduleId, DateTime archivedAt, CancellationToken cancellationToken = default);

    Task CreateLessonAsync(Lesson lesson, CancellationToken cancellationToken = default);

    Task<Lesson?> GetLessonAsync(Guid moduleId, Guid lessonId, CancellationToken cancellationToken = default);

    Task<List<LessonSummary>> GetLessonsAsync(Guid moduleId, CancellationToken cancellationToken = default);

    Task<int> GetLessonsCountAsync(Guid moduleId, CancellationToken cancellationToken = default);

    Task<int> GetNextLessonPositionAsync(Guid moduleId, CancellationToken cancellationToken = default);

    Task<bool> UpdateLessonAsync(Lesson updatedLesson, CancellationToken cancellationToken = default);

    Task<bool> ArchiveLessonAsync(Guid moduleId, Guid lessonId, DateTime archivedAt, CancellationToken cancellationToken = default);
}
