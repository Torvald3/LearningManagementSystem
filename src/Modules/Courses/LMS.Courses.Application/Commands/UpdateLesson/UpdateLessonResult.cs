using LMS.Courses.Application.Models;

namespace LMS.Courses.Application.Commands.UpdateLesson;

public record UpdateLessonResult(
    UpdateLessonStatus Status,
    Lesson? Lesson,
    IEnumerable<string> Errors);
