using LMS.Common.CQRS;

namespace LMS.Courses.Application.Commands.CreateLesson;

public record CreateLessonCommand(
    Guid CourseId,
    Guid ModuleId,
    string Title,
    string Content) : ICommand;
