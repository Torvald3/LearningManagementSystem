using LMS.Media.Core.Models;

namespace LMS.Media.Api.Models;

public record MediaResponse(
    Guid Id,
    MediaEntityType EntityType,
    Guid EntityId,
    string OriginalFileName,
    string ContentType,
    long Size,
    DateTime CreatedAt);
