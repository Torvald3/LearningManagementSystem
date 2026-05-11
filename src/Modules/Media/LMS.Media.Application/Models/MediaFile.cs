using LMS.Media.Core.Models;

namespace LMS.Media.Application.Models;

public record MediaFile(
    Guid Id,
    MediaEntityType EntityType,
    Guid EntityId,
    string ObjectKey,
    string OriginalFileName,
    string ContentType,
    long Size,
    DateTime CreatedAt);
