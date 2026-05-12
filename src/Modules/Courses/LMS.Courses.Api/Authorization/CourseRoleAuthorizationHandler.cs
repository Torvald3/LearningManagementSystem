using LMS.Common.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace LMS.Courses.Api.Authorization;

public class CourseRoleAuthorizationHandler : AuthorizationHandler<CourseRoleRequirement>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly ICourseAuthorizationService _courseAuthorizationService;

    public CourseRoleAuthorizationHandler(
        ICurrentUserService currentUserService,
        ICourseAuthorizationService courseAuthorizationService)
    {
        _currentUserService = currentUserService;
        _courseAuthorizationService = courseAuthorizationService;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        CourseRoleRequirement requirement)
    {
        if (_currentUserService.UserId is not { } userId)
        {
            return;
        }

        if (context.Resource is not HttpContext httpContext)
        {
            return;
        }

        var courseId = GetCourseId(httpContext);

        if (courseId is null)
        {
            return;
        }

        var hasRole = await _courseAuthorizationService.HasAnyRoleAsync(
            courseId.Value,
            userId,
            requirement.AllowedRoles.ToArray());

        if (hasRole)
        {
            context.Succeed(requirement);
        }
    }

    private static Guid? GetCourseId(HttpContext httpContext)
    {
        if (TryGetRouteGuid(httpContext, "courseId", out var courseId))
        {
            return courseId;
        }

        if (TryGetRouteGuid(httpContext, "id", out var id))
        {
            return id;
        }

        return null;
    }

    private static bool TryGetRouteGuid(HttpContext httpContext, string key, out Guid value)
    {
        value = Guid.Empty;

        var rawValue = httpContext.Request.RouteValues[key]?.ToString();

        return Guid.TryParse(rawValue, out value);
    }
}
