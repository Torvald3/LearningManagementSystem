using LMS.Common.Results;

namespace LMS.Users.Application.Errors;

public static class UserErrors
{
    public static Error UserNotFound(Guid userId) =>
        Error.NotFound(
            "users.user_not_found",
            $"User with id {userId} not found.");

    public static Error BioTooLong(int maxLength) =>
        Error.Validation(
            "users.bio_too_long",
            $"Bio must not exceed {maxLength} characters.");

    public static Error AvatarMediaIdInvalid =>
        Error.Validation(
            "users.avatar_media_id_invalid",
            "AvatarMediaId cannot be empty.");
}
