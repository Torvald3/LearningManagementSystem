using FluentValidation;
using LMS.Courses.Api.Models;

namespace LMS.Courses.Api.Validators;

public class UpdateCourseModuleRequestValidator : AbstractValidator<UpdateCourseModuleRequest>
{
    public UpdateCourseModuleRequestValidator()
    {
        RuleFor(r => r.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(r => r.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters.");

        RuleFor(r => r.Position)
            .GreaterThan(0).WithMessage("Position must be greater than 0.");
    }
}
