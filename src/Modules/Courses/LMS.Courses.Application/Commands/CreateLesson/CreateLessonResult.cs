using LMS.Courses.Application.Models;

namespace LMS.Courses.Application.Commands.CreateLesson;

public record CreateLessonResult(
    CreateLessonStatus Status,
    Lesson? Lesson,
    IEnumerable<string> Errors);
