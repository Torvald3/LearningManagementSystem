using LMS.Common.CQRS;

namespace LMS.Courses.Application.Commands.CreateCourse;

public record CreateCourseCommand(
    Guid AuthorId,
    string Title,
    string Theme,
    string Description) : ICommand;
    