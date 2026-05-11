using LMS.Common.Results;

namespace LMS.Identity.Application.Errors;

public static class IdentityErrors
{
    public static Error RegistrationFailed(IEnumerable<string> errors) =>
        Error.Validation(
            "identity.registration_failed",
            string.Join(" ", errors));

    public static Error InvalidCredentials =>
        Error.Validation(
            "identity.invalid_credentials",
            "Invalid email or password.");

    public static Error EmailNotConfirmed =>
        Error.Validation(
            "identity.email_not_confirmed",
            "Email is not confirmed.");

    public static Error UserNotFound(Guid userId) =>
        Error.NotFound(
            "identity.user_not_found",
            $"User with id {userId} not found.");

    public static Error EmailAlreadyConfirmed =>
        Error.Conflict(
            "identity.email_already_confirmed",
            "Email is already confirmed.");

    public static Error InvalidEmailConfirmationToken(IEnumerable<string> errors) =>
        Error.Validation(
            "identity.invalid_email_confirmation_token",
            string.Join(" ", errors));
}
