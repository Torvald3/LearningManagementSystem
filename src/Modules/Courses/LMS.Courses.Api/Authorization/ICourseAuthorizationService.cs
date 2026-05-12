using LMS.Courses.Core.Models;

namespace LMS.Courses.Api.Authorization;

public interface ICourseAuthorizationService
{
    Task<bool> HasAnyRoleAsync(Guid courseId, Guid userId, params CourseRole[] roles);
}
