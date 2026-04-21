using FluentValidation;
using LMS.Courses.Api.Models;

namespace LMS.Courses.Api.Validators;

public class CreateCourseRequestValidator : AbstractValidator<CreateCourseRequest>
{
    public CreateCourseRequestValidator()
    {
        RuleFor(r => r.Title)
            .NotEmpty().WithMessage("Title is required.");
        
        RuleFor(r => r.Description)
            .NotEmpty().WithMessage("Description is required.");
        
        RuleFor(r => r.Theme)
            .NotEmpty().WithMessage("Theme is required.");
    }
}