using LMS.Media.Core.Models;

namespace LMS.Media.Core.Services;

public interface IMediaStorage
{
    Task UploadAsync(
        string objectKey,
        Stream content,
        long size,
        string contentType,
        CancellationToken cancellationToken = default);

    Task<MediaUrl> GetReadUrlAsync(
        string objectKey,
        CancellationToken cancellationToken = default);
}
