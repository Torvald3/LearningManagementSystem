using LMS.Common.CQRS;

namespace LMS.Courses.Application.Commands.UpdateCourse;

public record UpdateCourseCommand(
    Guid CourseId,
    string Title,
    string Theme,
    string Description) : ICommand;