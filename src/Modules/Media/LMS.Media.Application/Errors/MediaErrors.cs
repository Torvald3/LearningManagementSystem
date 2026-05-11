using LMS.Common.Results;

namespace LMS.Media.Application.Errors;

public static class MediaErrors
{
    public static Error EntityIdRequired =>
        Error.Validation(
            "media.entity_id_required",
            "EntityId is required.");

    public static Error FileEmpty =>
        Error.Validation(
            "media.file_empty",
            "File is empty.");

    public static Error FileTooLarge(int maxSizeMb) =>
        Error.Validation(
            "media.file_too_large",
            $"File size must not exceed {maxSizeMb} MB.");

    public static Error NotFound(Guid mediaId) =>
        Error.NotFound(
            "media.not_found",
            $"Media file with id {mediaId} not found.");
}
