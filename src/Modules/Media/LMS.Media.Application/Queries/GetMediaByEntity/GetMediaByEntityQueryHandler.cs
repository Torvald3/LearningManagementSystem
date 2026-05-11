using LMS.Common.CQRS;
using LMS.Media.Application.Models;
using LMS.Media.Core.Services;
using Microsoft.Extensions.Logging;

namespace LMS.Media.Application.Queries.GetMediaByEntity;

public class GetMediaByEntityQueryHandler : IQueryHandler<GetMediaByEntityQuery, IReadOnlyList<MediaFile>>
{
    private readonly IMediaService _mediaService;
    private readonly ILogger<GetMediaByEntityQueryHandler> _logger;

    public GetMediaByEntityQueryHandler(
        IMediaService mediaService,
        ILogger<GetMediaByEntityQueryHandler> logger)
    {
        _mediaService = mediaService;
        _logger = logger;
    }

    public async Task<IReadOnlyList<MediaFile>> Handle(
        GetMediaByEntityQuery query,
        CancellationToken cancellationToken = default)
    {
        var mediaFiles = await _mediaService.GetMediaByEntityAsync(
            query.EntityType,
            query.EntityId,
            cancellationToken);

        _logger.LogInformation(
            "timestamp={Timestamp} level={Level} event={Event} entity_type={EntityType} entity_id={EntityId} media_count={MediaCount}",
            DateTime.UtcNow,
            "INFO",
            "media.get_by_entity.succeeded",
            query.EntityType,
            query.EntityId,
            mediaFiles.Count);

        return mediaFiles
            .Select(mediaFile => new MediaFile(
                mediaFile.Id,
                mediaFile.EntityType,
                mediaFile.EntityId,
                mediaFile.ObjectKey,
                mediaFile.OriginalFileName,
                mediaFile.ContentType,
                mediaFile.Size,
                mediaFile.CreatedAt))
            .ToList();
    }
}
