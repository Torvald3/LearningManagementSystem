namespace LMS.Courses.Api.Models;

public record AddCourseMemberRequest(
    Guid UserId,
    string Role);
