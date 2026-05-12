using LMS.Common.CQRS;
using LMS.Courses.Core.Models;

namespace LMS.Courses.Application.Commands.AddCourseMember;

public record AddCourseMemberCommand(
    Guid CourseId,
    Guid UserId,
    CourseRole Role) : ICommand;
