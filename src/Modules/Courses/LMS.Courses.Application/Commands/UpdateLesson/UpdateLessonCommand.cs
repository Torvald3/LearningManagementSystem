using LMS.Common.CQRS;

namespace LMS.Courses.Application.Commands.UpdateLesson;

public record UpdateLessonCommand(
    Guid CourseId,
    Guid ModuleId,
    Guid LessonId,
    string Title,
    string Content,
    int Position) : ICommand;
