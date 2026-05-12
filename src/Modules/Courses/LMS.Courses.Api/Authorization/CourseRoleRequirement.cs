using LMS.Courses.Core.Models;
using Microsoft.AspNetCore.Authorization;

namespace LMS.Courses.Api.Authorization;

public class CourseRoleRequirement : IAuthorizationRequirement
{
    public CourseRoleRequirement(params CourseRole[] allowedRoles)
    {
        AllowedRoles = allowedRoles;
    }

    public IReadOnlyCollection<CourseRole> AllowedRoles { get; }
}
