using LMS.Common.CQRS;
using LMS.Common.Results;
using LMS.Courses.Application.Errors;
using LMS.Courses.Core.Services;
using Microsoft.Extensions.Logging;
using CourseMemberModel = LMS.Courses.Application.Models.CourseMember;

namespace LMS.Courses.Application.Queries.GetCourseMembers;

public class GetCourseMembersQueryHandler : IQueryHandler<GetCourseMembersQuery, List<CourseMemberModel>>
{
    private readonly ICoursesService _coursesService;
    private readonly ILogger<GetCourseMembersQueryHandler> _logger;

    public GetCourseMembersQueryHandler(
        ICoursesService coursesService,
        ILogger<GetCourseMembersQueryHandler> logger)
    {
        _coursesService = coursesService;
        _logger = logger;
    }

    public async Task<Result<List<CourseMemberModel>>> Handle(
        GetCourseMembersQuery query,
        CancellationToken cancellationToken = default)
    {
        var course = await _coursesService.GetCourseAsync(query.CourseId, cancellationToken);

        if (course is null)
        {
            return CourseErrors.CourseNotFound(query.CourseId);
        }

        var members = await _coursesService.GetCourseMembersAsync(query.CourseId, cancellationToken);

        _logger.LogInformation(
            "timestamp={Timestamp} level={Level} event={Event} course_id={CourseId} members_count={MembersCount}",
            DateTime.UtcNow,
            "INFO",
            "course_members.get.succeeded",
            query.CourseId,
            members.Count);

        List<CourseMemberModel> result = members
            .Select(member => new CourseMemberModel(
                member.Id,
                member.CourseId,
                member.UserId,
                member.Role,
                member.CreatedAt,
                member.UpdatedAt))
            .ToList();

        return result;
    }
}
