using LMS.Media.Core.Models;

namespace LMS.Media.Core.Services;

public interface IMediaService
{
    Task CreateMediaAsync(MediaFile mediaFile, CancellationToken cancellationToken = default);

    Task<MediaFile?> GetMediaAsync(Guid mediaId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MediaFile>> GetMediaByEntityAsync(
        MediaEntityType entityType,
        Guid entityId,
        CancellationToken cancellationToken = default);

    Task<bool> ArchiveMediaAsync(Guid mediaId, DateTime archivedAt, CancellationToken cancellationToken = default);
}
