using LMS.Common.CQRS;

namespace LMS.Courses.Application.Commands.DeleteCourse;

public record DeleteCourseCommand(Guid CourseId) : ICommand;