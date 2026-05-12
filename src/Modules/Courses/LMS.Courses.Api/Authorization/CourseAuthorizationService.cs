using LMS.Courses.Core.Models;
using LMS.Courses.Core.Services;

namespace LMS.Courses.Api.Authorization;

public class CourseAuthorizationService : ICourseAuthorizationService
{
    private readonly ICoursesService _coursesService;

    public CourseAuthorizationService(ICoursesService coursesService)
    {
        _coursesService = coursesService;
    }

    public async Task<bool> HasAnyRoleAsync(Guid courseId, Guid userId, params CourseRole[] roles)
    {
        var member = await _coursesService.GetCourseMemberAsync(courseId, userId);

        return member is not null && roles.Contains(member.Role);
    }
}
