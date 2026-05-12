using FluentValidation;
using LMS.Courses.Api.Models;
using LMS.Courses.Core.Models;

namespace LMS.Courses.Api.Validators;

public class AddCourseMemberRequestValidator : AbstractValidator<AddCourseMemberRequest>
{
    public AddCourseMemberRequestValidator()
    {
        RuleFor(r => r.UserId)
            .NotEmpty().WithMessage("UserId is required.");

        RuleFor(r => r.Role)
            .NotEmpty().WithMessage("Role is required.")
            .Must(BeValidCourseRole).WithMessage("Role must be CourseOwner, Teacher, or Student.");
    }

    private static bool BeValidCourseRole(string role)
    {
        return !int.TryParse(role, out _) &&
               Enum.TryParse<CourseRole>(role, ignoreCase: true, out _);
    }
}
