using FluentValidation;
using LMS.Users.Api.Models;

namespace LMS.Users.Api.Validators;

public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        RuleFor(x => x.Bio)
            .MaximumLength(1024).WithMessage("Bio must not exceed 1024 characters.");

        RuleFor(x => x.AvatarMediaId)
            .NotEqual(Guid.Empty)
            .When(x => x.AvatarMediaId.HasValue)
            .WithMessage("AvatarMediaId cannot be empty.");
    }
}
