using LMS.Common.CQRS;

namespace LMS.Courses.Application.Commands.RemoveCourseMember;

public record RemoveCourseMemberCommand(
    Guid CourseId,
    Guid UserId) : ICommand;
